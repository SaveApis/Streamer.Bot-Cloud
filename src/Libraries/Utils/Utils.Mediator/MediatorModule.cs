using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Utils.Core.Infrastructure.Helpers;

namespace Utils.Mediator;

public class MediatorModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.AddMediatR(configuration =>
        {
            configuration.Lifetime = ServiceLifetime.Scoped;
            configuration.RegisterGenericHandlers = true;
            configuration.AutoRegisterRequestProcessors = true;
            configuration.RegisterServicesFromAssemblies(assemblyHelper.GetAssemblies().ToArray());
        });

        builder.Populate(collection);
    }
}
