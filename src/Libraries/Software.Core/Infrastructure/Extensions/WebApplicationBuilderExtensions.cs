using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Utils.Core;
using Utils.Core.Application.Helpers;
using Utils.Core.Infrastructure.Helpers;
using Utils.Correlation;
using Utils.Encryption;
using Utils.EntityFrameworkCore;
using Utils.Hangfire;
using Utils.Jwt;
using Utils.Mediator;
using Utils.Rest;
using Utils.Serilog;
using Utils.Swagger;
using Utils.Validation;

namespace Software.Core.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureSaveApis(this WebApplicationBuilder builder, Assembly softwareAssembly, Action<HostBuilderContext, ContainerBuilder, IAssemblyHelper>? registerSoftwareModulesAction = null)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var utilAssemblies = ReadUtilAssemblies();

        var assemblyHelper = new AssemblyHelper([softwareAssembly, currentAssembly, .. utilAssemblies]);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
                {
                    containerBuilder.RegisterModule(new CoreModule(assemblyHelper));
                    if (EF.IsDesignTime)
                    {
                        containerBuilder.RegisterModule(new EntityFrameworkCoreModule(assemblyHelper, context.Configuration));
                    }
                    else
                    {
                        containerBuilder.RegisterModule(new RestModule(assemblyHelper));
                        containerBuilder.RegisterModule<CorrelationModule>();
                        containerBuilder.RegisterModule<SwaggerModule>();
                        containerBuilder.RegisterModule(new JwtModule(context.Configuration, context.HostingEnvironment));

                        containerBuilder.RegisterModule(new MediatorModule(assemblyHelper));
                        containerBuilder.RegisterModule(new ValidationModule(assemblyHelper));
                        containerBuilder.RegisterModule(new HangfireModule(context.Configuration));
                        containerBuilder.RegisterModule(new EntityFrameworkCoreModule(assemblyHelper, context.Configuration));
                        containerBuilder.RegisterModule(new EncryptionModule(context.Configuration));
                        containerBuilder.RegisterModule(new SerilogModule(context.Configuration));

                        registerSoftwareModulesAction?.Invoke(context, containerBuilder, assemblyHelper);
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
        yield return typeof(JwtModule).Assembly;

        yield return typeof(MediatorModule).Assembly;
        yield return typeof(ValidationModule).Assembly;
        yield return typeof(HangfireModule).Assembly;
        yield return typeof(EntityFrameworkCoreModule).Assembly;
        yield return typeof(EncryptionModule).Assembly;
        yield return typeof(SerilogModule).Assembly;
    }
}
