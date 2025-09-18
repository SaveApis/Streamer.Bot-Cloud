namespace Utils.Encryption.Infrastructure.Services.Encryption;

public interface IEncryptionService
{
    string Encrypt(string plainText, string? existingIv, out string iv);
    string Decrypt(string cipherText, string iv);
}
