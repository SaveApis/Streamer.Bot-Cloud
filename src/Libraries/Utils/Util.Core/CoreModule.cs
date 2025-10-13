using Autofac;
using Util.Core.Application.Helpers;

namespace Util.Core;

public class CoreModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(assemblyHelper)
            .As<IAssemblyHelper>()
            .SingleInstance();
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => !t.IsAbstract)
            .AsImplementedInterfaces();
    }
}
