using System.Reflection;
using Utils.Core.Infrastructure.Helpers;

namespace Utils.Core.Application.Helpers;

public class AssemblyHelper(IEnumerable<Assembly> assemblies) : IAssemblyHelper
{
    private readonly IList<Assembly> _assemblies = assemblies.Distinct().ToList();

    public IReadOnlyCollection<Assembly> GetAssemblies()
    {
        return _assemblies.AsReadOnly();
    }
}
