using System.Reflection;

namespace Utils.Core.Application.Helpers;

public interface IAssemblyHelper
{
    IReadOnlyList<Assembly> GetAssemblies();
}