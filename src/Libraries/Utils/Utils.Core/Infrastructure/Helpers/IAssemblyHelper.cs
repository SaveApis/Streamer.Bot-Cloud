using System.Reflection;

namespace Utils.Core.Infrastructure.Helpers;

public interface IAssemblyHelper
{
    IReadOnlyCollection<Assembly> GetAssemblies();
}
