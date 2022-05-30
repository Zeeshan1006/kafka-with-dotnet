namespace Wikimedia.Producer.Service;

public interface ISendMessage
{
    Task<bool> SendMessageRequest(string key, string message);
}
