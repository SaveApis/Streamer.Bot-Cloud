using Autofac;
using Utils.Core.Infrastructure.Helpers;

namespace Utils.Core;

public class CoreModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(assemblyHelper)
            .As<IAssemblyHelper>()
            .SingleInstance();
    }
}
