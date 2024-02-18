using System.Data.SqlClient;
using AccountsSyncData.Models;

namespace AccountsSyncData.Handler;

public class MemberDetailChangedHandler : IMessageHandler
{
    public async Task HandleAsync(IMessage message)
    {
        var memberDetail = (MemberDetailChanged)message;
        
        
        var myConnection = new SqlConnection("user id=usernamehere;" +
                                             "password=passwordhere;server=serveraddresshere;" +
                                             "database=test; " +
                                             "connection timeout=30");
        await myConnection.BeginTransactionAsync();
        //save in db
    }

    // public static Type MessageType => typeof(MemberDetailChanged);
    public Type MessageType_()
    {
        return typeof(MemberDetailChanged);
    }
}