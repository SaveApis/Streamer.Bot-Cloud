namespace Utils.Rest.Infrastructure;

public interface IKnownException
{
    string ErrorCode { get; }
    string ErrorMessage { get; }
}
