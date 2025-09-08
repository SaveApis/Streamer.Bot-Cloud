using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Utils.Core;
using Utils.Core.Application.Helpers;
using Utils.Correlation;
using Utils.Hangfire;
using Utils.Hangfire.Application.Extensions;
using Utils.Hangfire.Domain.Types;
using Utils.Mediator;
using Utils.Rest;
using Utils.Swagger;

namespace Software.Core.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureSaveApis(this WebApplicationBuilder builder, Assembly softwareAssembly, out ApplicationType applicationType)
    {
        applicationType = builder.Configuration.GetHangfireApplicationType();
        var currentAssembly = Assembly.GetExecutingAssembly();
        var utilAssemblies = ReadUtilAssemblies();

        var assemblyHelper = new AssemblyHelper([softwareAssembly, currentAssembly, .. utilAssemblies]);
        
        var tmpApplicationType = applicationType;

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((_, containerBuilder) =>
                {
                    switch (tmpApplicationType)
                    {
                        case ApplicationType.Backend:
                            containerBuilder.RegisterModule(new CoreModule(assemblyHelper));

                            containerBuilder.RegisterModule(new RestModule(assemblyHelper));
                            containerBuilder.RegisterModule<CorrelationModule>();
                            containerBuilder.RegisterModule<SwaggerModule>();

                            containerBuilder.RegisterModule(new MediatorModule(assemblyHelper));
                            containerBuilder.RegisterModule(new HangfireModule(assemblyHelper, tmpApplicationType));
                            break;
                        case ApplicationType.Server:
                            containerBuilder.RegisterModule(new CoreModule(assemblyHelper));

                            containerBuilder.RegisterModule<CorrelationModule>();

                            containerBuilder.RegisterModule(new MediatorModule(assemblyHelper));
                            containerBuilder.RegisterModule(new HangfireModule(assemblyHelper, tmpApplicationType));
                            break;
                        case ApplicationType.Worker:
                            containerBuilder.RegisterModule(new CoreModule(assemblyHelper));

                            containerBuilder.RegisterModule<CorrelationModule>();

                            containerBuilder.RegisterModule(new MediatorModule(assemblyHelper));
                            containerBuilder.RegisterModule(new HangfireModule(assemblyHelper, tmpApplicationType));
                            break;
                        case ApplicationType.Unknown:
                            containerBuilder.RegisterModule(new CoreModule(assemblyHelper));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(tmpApplicationType), "Unhandled application type: " + tmpApplicationType);
                    }
                }
            );
    }

    private static IEnumerable<Assembly> ReadUtilAssemblies()
    {
        yield return typeof(CoreModule).Assembly;

        yield return typeof(RestModule).Assembly;
        yield return typeof(CorrelationModule).Assembly;
        yield return typeof(SwaggerModule).Assembly;

        yield return typeof(MediatorModule).Assembly;
        yield return typeof(HangfireModule).Assembly;
    }
}
