using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Utils.Encryption.Domain.Options;

namespace Utils.Encryption.Infrastructure.Services.Encryption;

public class EncryptionService(IOptionsMonitor<EncryptionOption> monitor) : IEncryptionService
{
    private EncryptionOption Option => monitor.CurrentValue;

    public string Encrypt(string plainText, string? existingIv, out string iv)
    {
        byte[] ivBytes;
        byte[] encryptedBytes;

        using (var aes = Aes.Create()) {
            aes.Key = Convert.FromBase64String(Option.Key);
            if (string.IsNullOrEmpty(existingIv))
            {
                aes.GenerateIV();
            }
            else
            {
                aes.IV = Convert.FromBase64String(existingIv);
            }

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    ivBytes = aes.IV;
                    encryptedBytes = ms.ToArray();
                }
            }
        }
        
        iv = Convert.ToBase64String(ivBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string cipherText, string iv)
    {
        string? plainText;

        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(Option.Key);
            aes.IV = Convert.FromBase64String(iv);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        plainText = sr.ReadToEnd();
                    }
                }
            }
        }

        return plainText;
    }
}
