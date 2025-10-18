using System.Reflection;

namespace Utils.Core.Application.Helpers;

public class AssemblyHelper(params Assembly[] assemblies) : IAssemblyHelper
{
    private readonly List<Assembly> _assemblies = [Assembly.GetExecutingAssembly(), ..assemblies.Distinct()];

    public IReadOnlyList<Assembly> GetAssemblies()
    {
        return _assemblies.AsReadOnly();
    }
}
