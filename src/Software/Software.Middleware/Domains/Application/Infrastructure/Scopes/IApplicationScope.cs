namespace Software.Middleware.Domains.Application.Infrastructure.Scopes;

public interface IApplicationScope
{
    public string Key { get; }
    public string Name { get; }
    public string? Description { get; }
}
