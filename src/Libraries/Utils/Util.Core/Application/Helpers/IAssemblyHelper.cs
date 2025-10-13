using System.Reflection;

namespace Util.Core.Application.Helpers;

public interface IAssemblyHelper
{
    IReadOnlyList<Assembly> GetAssemblies();
}
