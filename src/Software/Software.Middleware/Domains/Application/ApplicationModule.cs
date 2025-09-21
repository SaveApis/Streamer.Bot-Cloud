using Autofac;
using Autofac.Extensions.DependencyInjection;
using Software.Middleware.Domains.Application.Domain.Options;
using Software.Middleware.Domains.Application.Infrastructure.Scopes;
using Module = Autofac.Module;

namespace Software.Middleware.Domains.Application;

public class ApplicationModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterOption(builder);

        builder.RegisterAssemblyTypes(ThisAssembly)
            .Where(x => x.IsAssignableTo<IApplicationScope>())
            .AsImplementedInterfaces();
    }
    private void RegisterOption(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.Configure<ApplicationOptionsList>(configuration.GetSection("Applications"));

        builder.Populate(collection);
    }
}
