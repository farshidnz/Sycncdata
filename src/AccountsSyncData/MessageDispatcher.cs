using System.Reflection;
using AccountsSyncData.Handler;
using AccountsSyncData.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AccountsSyncData;

public class MessageDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly Dictionary<string, Type> _messageMappings = new()
    {
        { nameof(MemberDetailChanged), typeof(MemberDetailChanged) },
        { nameof(MemberJoined), typeof(MemberJoined) }
    };

    private readonly Dictionary<string, Func<IServiceProvider, IMessageHandler>> _handlers  = new()
    {
        { nameof(MemberDetailChanged), provider => provider.GetRequiredService<MemberDetailChangedHandler>() }
    };

    public MessageDispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _messageMappings = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessage).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary(info => info.Name, info => info.AsType());

        _handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToDictionary<TypeInfo, string, Func<IServiceProvider, IMessageHandler>>(
                info => ((Type) info.GetProperty(nameof(IMessageHandler.MessageType_))!.GetValue(null)!)!.Name,
                info => provider => (IMessageHandler) provider.GetRequiredService(info.AsType()));
    }

    public async Task DispatchAsync<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        using var scope = _scopeFactory.CreateScope();
        var handler = _handlers[message.MessageTypeName](scope.ServiceProvider);
        await handler.HandleAsync(message);
    }

    public bool CanHandleMessageType(string messageTypeName)
    {
        return _handlers.ContainsKey(messageTypeName);
    }

    public Type? GetMessageTypeByName(string messageTypeName)
    {
        return _messageMappings.GetValueOrDefault(messageTypeName);
    }
}