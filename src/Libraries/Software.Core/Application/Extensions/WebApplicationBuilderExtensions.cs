using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Util.Core;
using Util.Core.Application.Helpers;

namespace Software.Core.Application.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void UseAutofac(this WebApplicationBuilder builder, Assembly softwareAssembly)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();
        var utilAssemblies = GetUtilAssemblies();

        var assemblyHelper = new AssemblyHelper([softwareAssembly, thisAssembly, ..utilAssemblies]);
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, containerBuilder) => containerBuilder.RegisterModule(new CoreModule(assemblyHelper)));
    }

    private static List<Assembly> GetUtilAssemblies()
    {
        return new List<Assembly>();
    }
}
