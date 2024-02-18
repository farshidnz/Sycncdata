using System.Data.SqlClient;
using AccountSyncData.Consumer.Models;

namespace AccountSyncData.Consumer.Handler;

public class MemberDetailChangedHandler : IMessageHandler
{
    private readonly ILogger _logger;

    public MemberDetailChangedHandler(ILogger<MemberDetailChangedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(IMessage message)
    {
        _logger.LogInformation("MemberDetailChanged message recieved.");
        var memberDetail = (MemberDetailChanged)message;
        
        
        // var myConnection = new SqlConnection("user id=usernamehere;" +
        //                                      "password=passwordhere;server=serveraddresshere;" +
        //                                      "database=test; " +
        //                                      "connection timeout=30");
        // await myConnection.BeginTransactionAsync();
        //save in db
    }

    public static Type MessageType => typeof(MemberDetailChanged);
}