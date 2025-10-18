using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Utils.Core;
using Utils.Core.Application.Helpers;

namespace Software.Core.Application.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void UseAutofac(this WebApplicationBuilder builder, Assembly softwareAssembly, Action<IAssemblyHelper, HostBuilderContext, ContainerBuilder>? registerSoftwareModules = null)
    {
        var assemblyHelper = new AssemblyHelper(Assembly.GetExecutingAssembly(), softwareAssembly);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
                {
                    // Utils
                    containerBuilder.RegisterModule(new CoreModule(assemblyHelper));

                    // Software
                    registerSoftwareModules?.Invoke(assemblyHelper, context, containerBuilder);
                }
            );
    }
}
