using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Utils.Core;
using Utils.Core.Application.Helpers;
using Utils.Correlation;
using Utils.Mediator;
using Utils.Rest;
using Utils.Swagger;
using Utils.Validation;

namespace Software.Core.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureSaveApis(this WebApplicationBuilder builder, Assembly softwareAssembly)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var utilAssemblies = ReadUtilAssemblies();

        var assemblyHelper = new AssemblyHelper([softwareAssembly, currentAssembly, .. utilAssemblies]);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((_, containerBuilder) =>
                {
                    containerBuilder.RegisterModule(new CoreModule(assemblyHelper));

                    containerBuilder.RegisterModule(new RestModule(assemblyHelper));
                    containerBuilder.RegisterModule<CorrelationModule>();
                    containerBuilder.RegisterModule<SwaggerModule>();

                    containerBuilder.RegisterModule(new MediatorModule(assemblyHelper));
                    containerBuilder.RegisterModule(new ValidationModule(assemblyHelper));
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
        yield return typeof(ValidationModule).Assembly;
    }
}
