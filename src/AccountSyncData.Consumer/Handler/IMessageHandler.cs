using AccountSyncData.Consumer.Models;

namespace AccountSyncData.Consumer.Handler;

public interface IMessageHandler
{
    public Task HandleAsync(IMessage message);

    public static Type MessageType { get; }
}