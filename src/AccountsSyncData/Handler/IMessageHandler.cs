using AccountsSyncData.Models;

namespace AccountsSyncData.Handler;

public interface IMessageHandler
{
    public Task HandleAsync(IMessage message);
    // public static abstract Type MessageType { get; }
    public Type MessageType_();
}