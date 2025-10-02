using Software.Middleware.Domains.Application.Domain.VO;

namespace Software.Middleware.Domains.Application.Domain.Models.Entities;

public class ApplicationScopeEntity
{
    private ApplicationScopeEntity(string key, DateTime createdAt, DateTime? updatedAt, string name, string? description)
    {
        Key = key;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Name = name;
        Description = description;
    }

    public string Key { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }

    #region Entity

    public static ApplicationScopeEntity Create(ApplicationScopeKey key, string name, string? description = null)
    {
        return new ApplicationScopeEntity(key.Value, DateTime.UtcNow, null, name, description);
    }

    public bool HasChanges(string name, string? description)
    {
        return !Name.Equals(name)
               || !string.Equals(Description, description);
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion Entity
}
