using Software.Middleware.Domains.Application.Domain.Types;
using Software.Middleware.Domains.Application.Domain.VO;
using Stateless;

namespace Software.Middleware.Domains.Application.Domain.Models.Entities;

public class ApplicationEntity
{
    private readonly IList<ApplicationScopeEntity> _scopes = [];

    private ApplicationEntity(string key, DateTime createdAt, DateTime? updatedAt, ApplicationState state, string name, string? description, string authId, string authSecret, string iv)
    {
        _stateMachine = new StateMachine<ApplicationState, ApplicationStateTrigger>(() => State, s => State = s);
        ConfigureStateMachine();

        Key = key;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        State = state;
        Name = name;
        Description = description;
        AuthId = authId;
        AuthSecret = authSecret;
        Iv = iv;
    }

    public string Key { get; }

    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public ApplicationState State { get; private set; }

    public string Name { get; private set; }
    public string? Description { get; private set; }

    public string AuthId { get; private set; }
    public string AuthSecret { get; private set; }
    public string Iv { get; private set; }

    public IReadOnlyList<ApplicationScopeEntity> Scopes => _scopes.AsReadOnly();

    #region StateMachine

    private readonly StateMachine<ApplicationState, ApplicationStateTrigger> _stateMachine;

    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(ApplicationState.Active)
            .Permit(ApplicationStateTrigger.Deactivate, ApplicationState.Inactive);
        _stateMachine.Configure(ApplicationState.Inactive)
            .Permit(ApplicationStateTrigger.Activate, ApplicationState.Active);
    }

    private enum ApplicationStateTrigger
    {
        Activate,
        Deactivate
    }

    #endregion StateMachine

    #region Entity

    public static ApplicationEntity Create(ApplicationKey key, string name, string? description, string authId, string encryptedAuthSecret, string iv, IReadOnlyList<ApplicationScopeEntity> scopes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(authId, nameof(authId));
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedAuthSecret, nameof(encryptedAuthSecret));
        ArgumentException.ThrowIfNullOrWhiteSpace(iv, nameof(iv));

        var entity = new ApplicationEntity(key.Value, DateTime.UtcNow, null, ApplicationState.Active, name, description, authId, encryptedAuthSecret, iv);
        entity.UpdateScopes(scopes);

        return entity;
    }

    public bool HasChanges(string name, string? description, string authId, string encryptedAuthSecret, IReadOnlyList<string> scopes)
    {
        return !Name.Equals(name)
               || !string.Equals(Description, description)
               || !AuthId.Equals(authId)
               || !AuthSecret.Equals(encryptedAuthSecret)
               || !Scopes.Select(s => s.Key).OrderBy(s => s).SequenceEqual(scopes.OrderBy(s => s));
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

    public void UpdateCredentials(string encryptedAuthId, string encryptedAuthSecret, string iv)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedAuthId, nameof(encryptedAuthId));
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedAuthSecret, nameof(encryptedAuthSecret));
        ArgumentException.ThrowIfNullOrWhiteSpace(iv, nameof(iv));

        AuthId = encryptedAuthId;
        AuthSecret = encryptedAuthSecret;
        Iv = iv;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScopes(IReadOnlyList<ApplicationScopeEntity> scopes)
    {
        _scopes.Clear();
        foreach (var scope in scopes)
        {
            _scopes.Add(scope);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        _stateMachine.Fire(ApplicationStateTrigger.Activate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        _stateMachine.Fire(ApplicationStateTrigger.Deactivate);
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion Entity
}
