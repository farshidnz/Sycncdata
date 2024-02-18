namespace AccountSyncData.Consumer.Models;

public class MemberDetailChanged :IMessage
{
    public string MessageTypeName => nameof(MemberDetailChanged);
    public string FirstName { get; set; }
    public string LastName { get; set; }
}