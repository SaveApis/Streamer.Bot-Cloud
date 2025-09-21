namespace Software.Middleware.Domains.Application.Domain.Models.Entities;

public class ApplicationScopeEntity
{
    #region Entity

    public bool HasChanges(string name)
    {
        return !string.Equals(Name, name, StringComparison.InvariantCultureIgnoreCase);
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }

    #endregion Entity

    private ApplicationScopeEntity(string key, string name)
    {
        Key = key;
        Name = name;
    }

    public string Key { get; }
    public string Name { get; private set; }

    public static ApplicationScopeEntity Create(string key, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new ApplicationScopeEntity(key, name);
    }
}
