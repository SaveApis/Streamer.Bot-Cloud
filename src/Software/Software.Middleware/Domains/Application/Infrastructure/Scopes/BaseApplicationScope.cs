namespace Software.Middleware.Domains.Application.Infrastructure.Scopes;

public abstract class BaseApplicationScope(string key, string name, string? description) : IApplicationScope
{
    public string Key { get; } = key;
    public string Name { get; } = name;
    public string? Description { get; } = description;
}
