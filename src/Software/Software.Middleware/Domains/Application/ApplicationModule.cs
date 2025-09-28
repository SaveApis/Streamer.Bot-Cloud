using Autofac;
using Autofac.Extensions.DependencyInjection;
using Software.Middleware.Domains.Application.Domain.Options;
using Module = Autofac.Module;

namespace Software.Middleware.Domains.Application;

public class ApplicationModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.Configure<ApplicationOptionsList>(configuration.GetSection("Applications"));

        builder.Populate(collection);
    }
}
