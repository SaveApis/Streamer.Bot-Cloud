using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Correlate;
using Hangfire.Pro.Redis;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Utils.Core.Infrastructure.Helpers;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Domain.Options;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Jobs;

namespace Utils.Hangfire;

public class HangfireModule(IAssemblyHelper assemblyHelper, IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(GlobalConfiguration.Configuration).As<IGlobalConfiguration>().SingleInstance();

        RegisterOptions(builder, configuration);
        RegisterJobs(builder, assemblyHelper);
        RegisterRecurringEvents(builder, assemblyHelper);
        RegisterServices(builder);

        builder.RegisterBuildCallback(scope => GlobalConfiguration.Configuration.UseAutofacActivator(scope));
        builder.RegisterBuildCallback(async scope =>
        {
            var mediator = scope.Resolve<IMediator>();

            var startedEvent = new ApplicationStartedEvent();
            await mediator.Publish(startedEvent, CancellationToken.None).ConfigureAwait(false);
        });
    }

    private static void RegisterOptions(ContainerBuilder builder, IConfiguration configuration)
    {
        var collection = new ServiceCollection();

        collection.Configure<HangfireOption>(configuration.GetSection("Hangfire"));

        builder.Populate(collection);
    }
    private static void RegisterJobs(ContainerBuilder builder, IAssemblyHelper assemblyHelper)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHangfireJob<>)))
            .AsImplementedInterfaces();
    }
    private static void RegisterRecurringEvents(ContainerBuilder builder, IAssemblyHelper assemblyHelper)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => !t.IsAbstract && t.IsAssignableTo<IHangfireRecurringEvent>())
            .AsImplementedInterfaces();
    }
    private static void RegisterServices(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.AddHangfire((provider, globalConfiguration) =>
        {
            var logger = provider.GetRequiredService<ILogger<HangfireModule>>();

            logger.LogInformation("Configuring Hangfire...");
            var monitor = provider.GetRequiredService<IOptionsMonitor<HangfireOption>>();
            var option = monitor.CurrentValue;
            var serializedOption = JsonSerializer.Serialize(option);
            logger.LogInformation("Hangfire configuration: {option}", serializedOption);

            var redisOptions = new RedisStorageOptions
            {
                Database = option.Redis.Database,
                Prefix = option.Redis.Prefix.EndsWith(':')
                    ? option.Redis.Prefix
                    : option.Redis.Prefix + ":",
                MaxDeletedListLength = option.Redis.MaxDeletedListLength,
                MaxStateHistoryLength = option.Redis.MaxStateHistoryLength,
                MaxSucceededListLength = option.Redis.MaxSucceededListLength,
            };

            globalConfiguration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            globalConfiguration.UseSimpleAssemblyNameTypeSerializer();
            globalConfiguration.UseRecommendedSerializerSettings(settings => settings.Converters.Add(new StringEnumConverter()));
            globalConfiguration.UseRedisMetrics();
            globalConfiguration.UseRedisStorage(option.Redis.ToString(), redisOptions);

            globalConfiguration.UseCorrelate(provider);
            globalConfiguration.UseBatches();
            globalConfiguration.UseThrottling();
        });
        collection.AddHangfireServer((provider, options) =>
        {
            var monitor = provider.GetRequiredService<IOptionsMonitor<HangfireOption>>();
            var option = monitor.CurrentValue;

            options.Queues = Enum.GetNames<HangfireQueue>().Select(it => it.ToLowerInvariant()).ToArray();
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            options.ServerCheckInterval = TimeSpan.FromSeconds(10);
            options.ServerName = $"{Environment.MachineName}-{Environment.ProcessId}";
            options.WorkerCount = option.WorkerCount;
        });

        builder.Populate(collection);
    }
}
