namespace Utils.Encryption.Infrastructure.Services.Hashing;

public interface IHashingService
{
    string Hash(string input);
    bool Verify(string input, string hash);
}
