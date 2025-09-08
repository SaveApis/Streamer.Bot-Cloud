using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Correlate;
using Hangfire.Pro.Redis;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Utils.Core.Infrastructure.Helpers;
using Utils.Hangfire.Application.Events;
using Utils.Hangfire.Application.Handler.Authorization;
using Utils.Hangfire.Domain.Options;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Events;
using Utils.Hangfire.Infrastructure.Handlers.Authorization;
using Utils.Hangfire.Infrastructure.Jobs;
using Module = Autofac.Module;

namespace Utils.Hangfire;

public class HangfireModule(IAssemblyHelper assemblyHelper, ApplicationType applicationType) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => applicationType).As<ApplicationType>().SingleInstance();

        switch (applicationType)
        {
            case ApplicationType.Backend:
                RegisterAuthorization(builder, assemblyHelper);
                RegisterJobs(builder, assemblyHelper);
                RegisterHangfire(builder);

                RegisterEventCallback(builder, applicationType);
                break;
            case ApplicationType.Server:
                RegisterJobs(builder, assemblyHelper);
                RegisterRecurringEvents(builder, assemblyHelper);
                RegisterHangfire(builder);

                RegisterEventCallback(builder, applicationType);
                break;
            case ApplicationType.Worker:
                RegisterJobs(builder, assemblyHelper);
                RegisterRecurringEvents(builder, assemblyHelper);
                RegisterHangfire(builder);
                RegisterHangfireServer(builder);

                RegisterEventCallback(builder, applicationType);
                break;
            case ApplicationType.Unknown:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(applicationType), "Unhandled application type: " + applicationType);
        }
    }

    private static void RegisterAuthorization(ContainerBuilder containerBuilder, IAssemblyHelper assemblyHelper)
    {
        containerBuilder.RegisterType<DefaultDashboardAuthorizationFilter>().AsSelf();
        containerBuilder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(x => x.IsAssignableTo<IDashboardAuthorizationHandler>())
            .AsImplementedInterfaces();
    }
    private static void RegisterHangfire(ContainerBuilder containerBuilder)
    {
        var collection = new ServiceCollection();

        collection.AddHangfire((provider, globalConfiguration) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var hangfireOption = configuration.GetSection("Hangfire").Get<HangfireOption>() ?? throw new InvalidOperationException("Hangfire configuration is missing.");
                var storageOptions = new RedisStorageOptions
                {
                    Database = hangfireOption.Redis.Database,
                    Prefix = hangfireOption.Redis.Prefix.EndsWith(':')
                        ? hangfireOption.Redis.Prefix
                        : hangfireOption.Redis.Prefix + ":",
                    MaxSucceededListLength = hangfireOption.Dashboard.MaxSucceededListLength,
                    MaxDeletedListLength = hangfireOption.Dashboard.MaxDeletedListLength,
                    MaxStateHistoryLength = hangfireOption.Dashboard.MaxStateHistoryLength,
                };

                globalConfiguration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
                globalConfiguration.UseSimpleAssemblyNameTypeSerializer();
                globalConfiguration.UseRecommendedSerializerSettings(settings => settings.Converters.Add(new StringEnumConverter()));
                globalConfiguration.UseRedisMetrics();
                globalConfiguration.UseRedisStorage(hangfireOption.ToString(), storageOptions).WithJobExpirationTimeout(hangfireOption.ExpirationTimeout);

                globalConfiguration.UseBatches();
                globalConfiguration.UseThrottling();
                globalConfiguration.UseCorrelate(provider);
            }
        );

        containerBuilder.Populate(collection);

        containerBuilder.RegisterBuildCallback(scope => GlobalConfiguration.Configuration.UseAutofacActivator(scope));
    }
    private static void RegisterHangfireServer(ContainerBuilder containerBuilder)
    {
        var collection = new ServiceCollection();

        collection.AddHangfireServer((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var hangfireOption = configuration.GetSection("Hangfire").Get<HangfireOption>() ?? throw new InvalidOperationException("Hangfire configuration is missing.");

                options.Queues = Enum.GetNames<HangfireQueue>().Select(it => it.ToLowerInvariant()).ToArray();
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
                options.ServerName = $"{Environment.MachineName}-{Guid.NewGuid():N}";
                options.WorkerCount = hangfireOption.WorkerCount;
            }
        );

        containerBuilder.Populate(collection);
    }
    private static void RegisterJobs(ContainerBuilder containerBuilder, IAssemblyHelper assemblyHelper)
    {
        containerBuilder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJob<>)))
            .AsImplementedInterfaces();
    }
    private static void RegisterRecurringEvents(ContainerBuilder containerBuilder, IAssemblyHelper assemblyHelper)
    {
        containerBuilder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => t.IsAssignableTo<IRecurringHangfireEvent>())
            .AsImplementedInterfaces()
            .AsSelf();
    }
    private static void RegisterEventCallback(ContainerBuilder containerBuilder, ApplicationType applicationType)
    {
        containerBuilder.RegisterBuildCallback(async scope =>
        {
            var mediator = scope.Resolve<IMediator>();
            await mediator.Publish(new ApplicationStartedEvent(applicationType)).ConfigureAwait(false);
        });
    }
}
