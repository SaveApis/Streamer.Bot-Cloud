using System.Reflection;

namespace Util.Core.Application.Helpers;

public sealed class AssemblyHelper : IAssemblyHelper
{
    private readonly IList<Assembly> _assemblies = new List<Assembly>();

    public AssemblyHelper(params Assembly[] assemblies)
    {
        _assemblies.Clear();
        RegisterAssemblies([Assembly.GetExecutingAssembly(), ..assemblies]);
    }

    public IReadOnlyList<Assembly> GetAssemblies()
    {
        return _assemblies.AsReadOnly();
    }

    private void RegisterAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterAssembly(assembly);
        }
    }
    
    private void RegisterAssembly(Assembly assembly)
    {
        if (_assemblies.Contains(assembly))
        {
            return;
        }

        _assemblies.Add(assembly);
    }
}
