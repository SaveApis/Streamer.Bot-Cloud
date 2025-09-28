using Autofac;
using Utils.Core.Infrastructure.Helpers;
using Utils.Mapper.Infrastructure.Mapping;

namespace Utils.Mapper;

public class MapperModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(assemblyHelper.GetAssemblies().ToArray())
            .Where(x => !x.IsAbstract && x.IsAssignableTo<IMapper>())
            .AsImplementedInterfaces();
    }
}
