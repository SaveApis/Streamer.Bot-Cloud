using Hangfire.Throttling;
using MediatR;
using Software.Middleware.Domains.Application.Application.Events;
using Software.Middleware.Domains.Application.Application.Services.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.Models.Dto.ApplicationScope;
using Software.Middleware.Domains.Application.Domain.VO;
using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.EntityFrameworkCore.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("application:scopes:synchronize")]
public class SynchronizeApplicationScopesJob(ILogger logger, IApplicationScopeQueryService queryService, IApplicationScopeMutationService mutationService, IMediator mediator, IEnumerable<IApplicationScope> applicationScopes) : BaseHangfireJob<MigrationCompletedEvent>(logger)
{
    protected override bool CheckSupport(MigrationCompletedEvent @event)
    {
        return @event.DbContextType == typeof(CoreWriteDbContext);
    }

    [HangfireJobName("Synchronize application scopes")]
    public override async Task RunAsync(MigrationCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        var codeApplicationScopes = applicationScopes.ToList();
        var dbApplicationScopes = await queryService.GetAllAsync(cancellationToken);

        var codeKeys = codeApplicationScopes.Select(x => x.Key).ToHashSet();
        var dbKeys = dbApplicationScopes.Select(x => x.Key).ToHashSet();

        var toAdd = codeKeys.Except(dbKeys).ToList();
        var toRemove = dbKeys.Except(codeKeys).ToList();
        var toUpdate = codeKeys.Intersect(dbKeys).ToList();
        Logger.Debug("Found {ToAdd} application scopes to add, {ToRemove} to remove and {ToUpdate} to update", toAdd.Count, toRemove.Count, toUpdate.Count);

        var addTask = AddTask(toAdd, codeApplicationScopes, cancellationToken);
        var removeTask = RemoveTask(toRemove, cancellationToken);
        var updateTask = UpdateTask(toUpdate, codeApplicationScopes, cancellationToken);

        await Task.WhenAll(addTask, removeTask, updateTask);
        Logger.Information("Synchronized application scopes");

        await mediator.Publish(new ApplicationScopesSynchronizedEvent(), cancellationToken).ConfigureAwait(false);
    }

    private async Task AddTask(List<string> toAdd, List<IApplicationScope> codeApplicationScopes, CancellationToken cancellationToken = default)
    {
        var createDtoEnumerable = codeApplicationScopes
            .Where(x => toAdd.Contains(x.Key))
            .Select(x => new ApplicationScopeCreateDto
            {
                Key = x.Key,
                Name = x.Name,
                Description = x.Description,
            });
        var createTaskEnumerable = createDtoEnumerable.Select(dto => mutationService.CreateAsync(dto, cancellationToken));
        await Task.WhenAll(createTaskEnumerable).ConfigureAwait(false);
    }

    private async Task RemoveTask(List<string> toRemove, CancellationToken cancellationToken = default)
    {
        var deleteTaskEnumerable = toRemove.Select(key => mutationService.DeleteAsync(ApplicationScopeKey.From(key), cancellationToken));

        await Task.WhenAll(deleteTaskEnumerable).ConfigureAwait(false);
    }

    private async Task UpdateTask(List<string> toUpdate, List<IApplicationScope> codeApplicationScopes, CancellationToken cancellationToken = default)
    {
        var updateDtoEnumerable = codeApplicationScopes
            .Where(x => toUpdate.Contains(x.Key))
            .Select(x => new
            {
                x.Key,
                Dto = new ApplicationScopeUpdateDto
                {
                    Name = x.Name,
                    Description = x.Description,
                }
            });
        var updateTaskEnumerable = updateDtoEnumerable.Select(x => mutationService.UpdateAsync(ApplicationScopeKey.From(x.Key), x.Dto, cancellationToken));
        await Task.WhenAll(updateTaskEnumerable).ConfigureAwait(false);
    }
}
