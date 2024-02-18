using System.Data.SqlClient;
using AccountSyncData.Consumer.Models;

namespace AccountSyncData.Consumer.Handler;

public class MemberJoinedHandler : IMessageHandler
{
    private readonly ILogger _logger;

    public MemberJoinedHandler(ILogger<MemberJoinedHandler> logger)
    {
        _logger = logger;
    }
    public async Task HandleAsync(IMessage message)
    {
        _logger.LogInformation("MemberJoined message recieved.");
        //var memberDetail = (MemberJoined)message;
        
        //save in db
        //PII data
    }

    public static Type MessageType => typeof(MemberJoined);
}