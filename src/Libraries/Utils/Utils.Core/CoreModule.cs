using Autofac;
using Utils.Core.Application.Helpers;

namespace Utils.Core;

public class CoreModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .AsImplementedInterfaces();

        builder.RegisterInstance(assemblyHelper)
            .As<IAssemblyHelper>()
            .SingleInstance();
    }
}