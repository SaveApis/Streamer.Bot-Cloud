using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using Utils.Core.Infrastructure.Helpers;

namespace Utils.Core;

public class CoreModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(t => !t.IsAbstract)
            .AsImplementedInterfaces();
        builder.RegisterInstance(assemblyHelper)
            .As<IAssemblyHelper>()
            .SingleInstance();
    }
}
