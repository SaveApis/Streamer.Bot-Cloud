using System.Text;
using Software.Middleware.Domains.Application.Domain.Types;
using Utils.Encryption.Infrastructure.Services.Encryption;

namespace Software.Middleware.Domains.Application.Domain.Models.Entities;

public class ApplicationEntity
{
    #region Entity

    public bool HasChanges(string name, string authId, string authSecret, List<ApplicationScopeEntity> applicationScopes, IEncryptionService encryptionService)
    {
        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(authSecret));
        var encryptedSecret = encryptionService.Encrypt(normalSecret, Iv, out _);

        return !string.Equals(Name, name, StringComparison.InvariantCultureIgnoreCase)
               || !string.Equals(AuthId, authId, StringComparison.InvariantCultureIgnoreCase)
               || !string.Equals(AuthSecret, encryptedSecret, StringComparison.InvariantCultureIgnoreCase)
               || applicationScopes.Count != Scopes.Count
               || applicationScopes.Any(x => Scopes.All(s => s.Key != x.Key));
    }

    public void UpdateFull(string name, string authId, string authSecret, List<ApplicationScopeEntity> scopes, IEncryptionService encryptionService)
    {
        UpdateName(name);
        UpdateAuthId(authId);
        UpdateAuthSecret(authSecret, encryptionService);
        UpdateScopes(scopes);
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }

    public void UpdateAuthId(string authId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authId, nameof(authId));

        AuthId = authId;
    }

    public void UpdateAuthSecret(string authSecret, IEncryptionService encryptionService)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authSecret, nameof(authSecret));

        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(authSecret));
        var encryptedSecret = encryptionService.Encrypt(normalSecret, null, out var iv);

        AuthSecret = encryptedSecret;
        Iv = iv;
    }

    public void UpdateScopes(List<ApplicationScopeEntity> scopes)
    {
        Scopes.Clear();
        Scopes.AddRange(scopes);
    }

    public bool VerifySecret(string authSecret, IEncryptionService encryptionService)
    {
        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(authSecret));
        var encryptedSecret = encryptionService.Encrypt(normalSecret, Iv, out _);

        return string.Equals(AuthSecret, encryptedSecret, StringComparison.InvariantCultureIgnoreCase);
    }

    #endregion Entity

    private ApplicationEntity(string key, string name, ApplicationSource source, string authId, string authSecret, string iv)
    {
        Key = key;
        Name = name;
        Source = source;
        AuthId = authId;
        AuthSecret = authSecret;
        Iv = iv;
    }

    public string Key { get; }
    public string Name { get; private set; }
    public ApplicationSource Source { get; }

    internal string AuthId { get; private set; }
    internal string AuthSecret { get; private set; }

    internal string Iv { get; private set; }

    public List<ApplicationScopeEntity> Scopes { get; } = [];

    private static ApplicationEntity Create(string key, string name, ApplicationSource source, string authId, string authSecret, List<ApplicationScopeEntity> scopes, IEncryptionService encryptionService)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(authId, nameof(authId));
        ArgumentException.ThrowIfNullOrWhiteSpace(authSecret, nameof(authSecret));

        var normalSecret = Encoding.UTF8.GetString(Convert.FromBase64String(authSecret));
        var encryptedSecret = encryptionService.Encrypt(normalSecret, null, out var iv);

        var entity = new ApplicationEntity(key, name, source, authId, encryptedSecret, iv);
        entity.UpdateScopes(scopes);

        return entity;
    }

    public static ApplicationEntity CreateFromConfiguration(string key, string name, string authId, string authSecret, List<ApplicationScopeEntity> scopes, IEncryptionService encryptionService)
    {
        return Create(key, name, ApplicationSource.Configuration, authId, authSecret, scopes, encryptionService);
    }

    public static ApplicationEntity CreateFromApi(string key, string name, string authId, string authSecret, List<ApplicationScopeEntity> scopes, IEncryptionService encryptionService)
    {
        return Create(key, name, ApplicationSource.Api, authId, authSecret, scopes, encryptionService);
    }
}
