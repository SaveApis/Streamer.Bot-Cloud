using Hangfire.Throttling;
using Microsoft.Extensions.Options;
using Serilog;
using Software.Middleware.Domains.Application.Application.Events;
using Software.Middleware.Domains.Application.Application.Services.Application;
using Software.Middleware.Domains.Application.Domain.Models.Dto.Application;
using Software.Middleware.Domains.Application.Domain.Options;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Attributes;
using Utils.Hangfire.Infrastructure.Jobs;
using ILogger = Serilog.ILogger;

namespace Software.Middleware.Domains.Application.Application.Jobs;

[HangfireQueue(HangfireQueue.System)]
[Mutex("application:applications:seed")]
public class SeedConfiguredApplicationJob(ILogger logger, IOptionsMonitor<ApplicationOptionsList> monitor, IApplicationQueryService queryService, IApplicationMutationService mutationService)
    : BaseHangfireJob<ApplicationScopesSynchronizedEvent>(logger)
{
    public ApplicationOptionsList Options => monitor.CurrentValue;

    [HangfireJobName("Seed missing configured applications")]
    public override async Task RunAsync(ApplicationScopesSynchronizedEvent @event, CancellationToken cancellationToken = default)
    {
        var dbApplications = await queryService.GetAllAsync(cancellationToken).ConfigureAwait(false);

        var codeKeys = Options.Select(x => x.Key).ToHashSet();
        var dbKeys = dbApplications.Select(x => x.Key).ToHashSet();

        var missingKeys = codeKeys.Except(dbKeys).ToList();
        Logger.Debug("Seeding {Count} missing applications: {Keys}", missingKeys.Count, string.Join(", ", missingKeys));

        var createDtos = Options
            .Where(x => missingKeys.Contains(x.Key))
            .Select(x => new ApplicationCreateDto
            {
                Key = x.Key,
                Name = x.Name,
                Scopes = x.Scopes,
                AuthId = x.AuthId,
                AuthSecret = x.AuthSecret,
            }).ToList();
        var createTasks = createDtos.Select(x => mutationService.CreateAsync(x, cancellationToken));
        await Task.WhenAll(createTasks).ConfigureAwait(false);

        Logger.Information("Successfully seeded {Count} missing applications", createDtos.Count);
    }
}
