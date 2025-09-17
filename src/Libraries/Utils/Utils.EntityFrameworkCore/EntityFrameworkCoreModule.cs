using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.Core.Infrastructure.Helpers;
using Utils.EntityFrameworkCore.Domain.Options;
using Utils.EntityFrameworkCore.Infrastructure.Context;

namespace Utils.EntityFrameworkCore;

public class EntityFrameworkCoreModule(IAssemblyHelper assemblyHelper, IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(x => !x.IsAbstract && x.IsAssignableTo<DbContext>())
            .As<DbContext>();
        RegisterOptions(builder, configuration);
    }

    private static void RegisterOptions(ContainerBuilder builder, IConfiguration configuration)
    {
        var collection = new ServiceCollection();

        collection.Configure<MySqlOption>(configuration.GetSection("MySql"));

        builder.Populate(collection);
    }
}
