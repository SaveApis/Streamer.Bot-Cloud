using Hangfire.Throttling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Domain.Options;
using Software.Middleware.Domains.Application.Domain.Types;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.Encryption.Infrastructure.Services.Encryption;
using Utils.EntityFrameworkCore.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Software.Middleware.Domains.Application.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("core:applications:synchronize")]
public class SynchronizeConfiguredApplicationsJob(
    ILogger<IHangfireJob<MigrationCompletedEvent>> logger,
    IDbContextFactory<CoreWriteDbContext> factory,
    IEncryptionService encryptionService,
    IOptionsMonitor<ApplicationOptionsList> monitor) : BaseHangfireJob<MigrationCompletedEvent>(logger)
{
    private ApplicationOptionsList Options => monitor.CurrentValue;

    [HangfireJobName("Synchronize configured applications")]
    public override async Task RunAsync(MigrationCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        await using var context = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var dbApplications = await context.Applications.AsNoTracking()
            .Where(x => x.Source == ApplicationSource.Configuration)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var configurationApplicationKeys = Options.Select(x => x.Key).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
        var dbApplicationKeys = dbApplications.Select(x => x.Key).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        var keysToAdd = configurationApplicationKeys.Except(dbApplicationKeys).ToList();
        var keysToRemove = dbApplicationKeys.Except(configurationApplicationKeys).ToList();
        var keysToUpdate = configurationApplicationKeys.Intersect(dbApplicationKeys).ToList();
        Logger.LogInformation("Found {AddCount} applications to add, {RemoveCount} applications to remove and {UpdateCount} applications to update", keysToAdd.Count, keysToRemove.Count,
            keysToUpdate.Count);

        var addTask = AddAsync(context, keysToAdd, Options, encryptionService);
        var removeTask = RemoveAsync(context, keysToRemove);
        var updateTask = UpdateAsync(context, keysToUpdate, Options, encryptionService);
        await Task.WhenAll(addTask, removeTask, updateTask).ConfigureAwait(false);

        var count = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Logger.LogInformation("Synchronized total of {Count} applications", count);
    }

    private static async Task AddAsync(CoreWriteDbContext context, List<string> keys, ApplicationOptionsList options, IEncryptionService encryptionService)
    {
        if (keys.Count == 0)
        {
            return;
        }

        var applicationScopes = await context.ApplicationScopes.ToListAsync().ConfigureAwait(false);

        var optionsToAdd = options.Where(x => keys.Contains(x.Key, StringComparer.InvariantCultureIgnoreCase)).ToList();
        foreach (var option in optionsToAdd)
        {
            var selectedScopes = applicationScopes
                .Where(s => option.Scopes.Contains(s.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToList();

            var entity = ApplicationEntity.CreateFromConfiguration(option.Key, option.Name, option.AuthId, option.AuthSecret, selectedScopes, encryptionService);
            await context.Applications.AddAsync(entity).ConfigureAwait(false);
        }
    }

    private static async Task RemoveAsync(CoreWriteDbContext context, List<string> keys)
    {
        if (keys.Count == 0)
        {
            return;
        }

        var entitiesToRemove = await context.Applications
            .Include(a => a.Scopes)
            .Where(x => keys.Any(k => string.Equals(k, x.Key, StringComparison.InvariantCultureIgnoreCase)))
            .ToListAsync()
            .ConfigureAwait(false);
        context.Applications.RemoveRange(entitiesToRemove);
    }

    private static async Task UpdateAsync(CoreWriteDbContext context, List<string> keys, ApplicationOptionsList options, IEncryptionService encryptionService)
    {
        if (keys.Count == 0)
        {
            return;
        }

        var applicationScopes = await context.ApplicationScopes.ToListAsync().ConfigureAwait(false);

        var optionsToUpdate = options.Where(x => keys.Contains(x.Key, StringComparer.InvariantCultureIgnoreCase)).ToList();
        var entitiesToUpdate = await context.Applications
            .Where(x => keys.Any(k => string.Equals(k, x.Key, StringComparison.InvariantCultureIgnoreCase)))
            .Include(x => x.Scopes)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var entity in entitiesToUpdate)
        {
            var option = optionsToUpdate.First(x => string.Equals(x.Key, entity.Key, StringComparison.InvariantCultureIgnoreCase));
            var scopes = option.Scopes
                .Select(s => applicationScopes.Single(db => string.Equals(db.Key, s, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            if (entity.HasChanges(option.Name, option.AuthId, option.AuthSecret, scopes, encryptionService))
            {
                entity.UpdateFull(option.Name, option.AuthId, option.AuthSecret, scopes, encryptionService);
                context.Applications.Update(entity);
            }
        }
    }
}
