namespace Sdk.Messaging;

public interface IResponse
{
    public ErrorInfo? RequestError { get; }
}