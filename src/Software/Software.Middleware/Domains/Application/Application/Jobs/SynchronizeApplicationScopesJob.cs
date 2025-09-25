using Hangfire.Throttling;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Application.Events;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.EntityFrameworkCore.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Software.Middleware.Domains.Application.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("core:applications:scopes:synchronize")]
public class SynchronizeApplicationScopesJob(
    ILogger<IHangfireJob<MigrationCompletedEvent>> logger,
    IDbContextFactory<CoreWriteDbContext> factory,
    IMediator mediator,
    IEnumerable<IApplicationScope> scopes) : BaseHangfireJob<MigrationCompletedEvent>(logger)
{
    protected override bool CheckSupport(MigrationCompletedEvent @event)
    {
        return @event.DbContextType == typeof(CoreWriteDbContext);
    }

    [HangfireJobName("Synchronize application scopes")]
    public override async Task RunAsync(MigrationCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        var codeScopes = scopes.ToList();
        if (codeScopes.Count > 0)
        {
            await SynchronizeAsync(codeScopes, cancellationToken).ConfigureAwait(false);
        }

        await mediator.Publish(new ApplicationScopesSynchronizedEvent(), cancellationToken).ConfigureAwait(false);
    }

    private async Task SynchronizeAsync(List<IApplicationScope> codeScopes, CancellationToken cancellationToken)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var dbScopes = await context.ApplicationScopes
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var codeScopeKeys = codeScopes.Select(s => s.Key).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
        var dbScopeKeys = dbScopes.Select(s => s.Key).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        var keysToAdd = codeScopeKeys.Except(dbScopeKeys).ToList();
        var keysToRemove = dbScopeKeys.Except(codeScopeKeys).ToList();
        var keysToUpdate = codeScopeKeys.Intersect(dbScopeKeys).ToList();
        Logger.LogInformation("Found {AddCount} scopes to add, {RemoveCount} scopes to remove and {UpdateCount} scopes to update", keysToAdd.Count, keysToRemove.Count, keysToUpdate.Count);

        var addTask = AddAsync(context, keysToAdd, codeScopes);
        var removeTask = RemoveAsync(context, keysToRemove);
        var updateTask = UpdateAsync(context, keysToUpdate, codeScopes);
        await Task.WhenAll(addTask, removeTask, updateTask).ConfigureAwait(false);

        var count = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Logger.LogInformation("Synchronized total of {Count} application scopes", count);
    }

    private static async Task AddAsync(CoreWriteDbContext context, List<string> keysToAdd, List<IApplicationScope> codeScopes)
    {
        if (keysToAdd.Count == 0)
        {
            return;
        }

        var scopesToAdd = codeScopes
            .Where(s => keysToAdd.Contains(s.Key, StringComparer.InvariantCultureIgnoreCase))
            .Select(s => ApplicationScopeEntity.Create(s.Key, s.Name));

        await context.ApplicationScopes.AddRangeAsync(scopesToAdd).ConfigureAwait(false);
    }

    private static async Task RemoveAsync(CoreWriteDbContext context, List<string> keysToRemove)
    {
        if (keysToRemove.Count == 0)
        {
            return;
        }

        var scopesToRemove = await context.ApplicationScopes
            .Where(s => keysToRemove.Any(x => x.Equals(s.Key, StringComparison.InvariantCultureIgnoreCase)))
            .ToListAsync()
            .ConfigureAwait(false);

        context.ApplicationScopes.RemoveRange(scopesToRemove);
    }

    private static async Task UpdateAsync(CoreWriteDbContext context, List<string> keysToUpdate, List<IApplicationScope> codeScopes)
    {
        if (keysToUpdate.Count == 0)
        {
            return;
        }

        var scopesToUpdate = await context.ApplicationScopes
            .Where(s => keysToUpdate.Any(x => x.Equals(s.Key, StringComparison.InvariantCultureIgnoreCase)))
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var dbScope in scopesToUpdate)
        {
            var codeScope = codeScopes.First(s => s.Key.Equals(dbScope.Key, StringComparison.InvariantCultureIgnoreCase));
            if (!dbScope.HasChanges(codeScope.Name))
            {
                continue;
            }

            dbScope.UpdateName(codeScope.Name);
            context.ApplicationScopes.Update(dbScope);
        }
    }
}
