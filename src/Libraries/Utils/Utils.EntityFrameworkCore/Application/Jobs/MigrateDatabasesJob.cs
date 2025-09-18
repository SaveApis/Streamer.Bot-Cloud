using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utils.EntityFrameworkCore.Application.Events;
using Utils.EntityFrameworkCore.Infrastructure.Context;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.EntityFrameworkCore.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
public class MigrateDatabasesJob(ILogger<IHangfireJob<ApplicationStartedEvent>> logger, IMediator mediator, IEnumerable<DbContext> dbContexts) : BaseHangfireJob<ApplicationStartedEvent>(logger)
{
    protected override bool CheckSupport(ApplicationStartedEvent @event)
    {
        return dbContexts.OfType<IWriteDbContext>().Any();
    }

    [HangfireJobName("Migrate databases")]
    public override async Task RunAsync(ApplicationStartedEvent @event, CancellationToken cancellationToken = default)
    {
        var dbContextList = dbContexts.ToList();
        if (dbContextList.Count == 0)
        {
            Logger.LogInformation("No DbContext found to migrate.");
            return;
        }

        foreach (var dbContext in dbContextList)
        {
            var dbContextName = dbContext.GetType().Name;
            if (dbContext is not IWriteDbContext)
            {
                Logger.LogDebug("Skipping migration for DbContext {DbContextName} because it does not implement IWriteDbContext.", dbContextName);
                continue;
            }

            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            Logger.LogInformation("Migrated database for DbContext {DbContextName}.", dbContextName);

            await mediator.Publish(new MigrationCompletedEvent(dbContext.GetType()), cancellationToken).ConfigureAwait(false);
        }
    }
}
