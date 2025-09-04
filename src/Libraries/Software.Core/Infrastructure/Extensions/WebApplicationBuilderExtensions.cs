using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Utils.Core;
using Utils.Core.Application.Helpers;

namespace Software.Core.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureSaveApis(this WebApplicationBuilder builder, Assembly softwareAssembly)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var utilAssemblies = ReadUtilAssemblies();

        var assemblyHelper = new AssemblyHelper([softwareAssembly, currentAssembly, ..utilAssemblies]);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((_, containerBuilder) => containerBuilder.RegisterModule(new CoreModule(assemblyHelper)));
    }
    
    private static IEnumerable<Assembly> ReadUtilAssemblies()
    {
        yield return typeof(CoreModule).Assembly;
    }
}
