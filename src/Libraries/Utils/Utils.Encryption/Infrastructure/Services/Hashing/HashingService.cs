using BCrypt.Net;

namespace Utils.Encryption.Infrastructure.Services.Hashing;

public class HashingService : IHashingService
{
    public string Hash(string input)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();

        return BCrypt.Net.BCrypt.HashPassword(input, salt, true, HashType.SHA512);
    }

    public bool Verify(string input, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(input, hash, true, HashType.SHA512);
    }
}
