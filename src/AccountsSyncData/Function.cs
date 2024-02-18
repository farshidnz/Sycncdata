using System.Data.SqlClient;
using System.Reflection;
using System.Text.Json;
using AccountsSyncData.Handler;
using AccountsSyncData.Models;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AccountsSyncData;

public class Function
{
    
    private readonly MessageDispatcher _dispatcher;
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        
        Console.WriteLine("Setting up the DI container");
        // var serviceCollection = new ServiceCollection();
        // Console.WriteLine("Adding a scoped service");
        // serviceCollection.AddSingleton<MessageDispatcher>();
        //
        //
        // serviceCollection.AddMessageHandlers();
    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        Console.WriteLine("Starting messages" );
        // context.Logger.LogInformation($"Number of events: {evnt.Records.Count}");
        foreach(var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        // context.Logger.LogInformation($"Processed message {message.Body}");
        var messageTypeName = message.MessageAttributes
            .GetValueOrDefault(nameof(IMessage.MessageTypeName))?.StringValue;

        if (messageTypeName is null)
        {
            // context.Logger.LogInformation($"Processed message {message.MessageId}. No type was found.");
            return;
        }

        if (!_dispatcher.CanHandleMessageType(messageTypeName))
        {
            // context.Logger.LogError($"Unable to process message {message.MessageId}.");
            return;
        }

        var messageType = _dispatcher.GetMessageTypeByName(messageTypeName)!;

        var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;

        try
        {
            await _dispatcher.DispatchAsync(messageAsType);
        }
        catch (Exception e)
        {
            // context.Logger.LogError($"Failed to handle Message of type {messageTypeName} , MessageId : {message.MessageId}. {e.Message}");
        }
        
        // TODO: Do interesting work based on the new message
        await Task.CompletedTask;
    }
}