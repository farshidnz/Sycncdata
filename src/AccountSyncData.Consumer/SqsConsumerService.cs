using System.Net;
using System.Text.Json;
using AccountSyncData.Consumer.Models;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AccountSyncData.Consumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly MessageDispatcher _dispatcher;
    private const string QueueName = "wearemc-accounts-sync-data-Member";
    private readonly List<string> _messageAttributeNames = new() { "All" };
    private readonly ILogger _logger;

    public SqsConsumerService(IAmazonSQS sqs, MessageDispatcher dispatcher, ILogger<SqsConsumerService> logger)
    {
        _sqs = sqs;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting the sqs service.");
        
        var queueUrl = await _sqs.GetQueueUrlAsync(QueueName, ct);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _messageAttributeNames
        };
        while (!ct.IsCancellationRequested)
        {
            var messageResponse = await _sqs.ReceiveMessageAsync(receiveRequest, ct);
            if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                //Do some logging or handling?
                continue;
            }

            foreach (var message in messageResponse.Messages)
            {
                _logger.LogInformation($"Found a message. {message.MessageId}");
                
                var messageTypeName = message.MessageAttributes
                    .GetValueOrDefault(nameof(IMessage.MessageTypeName))?.StringValue;

                if (messageTypeName is null)
                {
                    continue;
                }

                if (!_dispatcher.CanHandleMessageType(messageTypeName))
                {
                    continue;
                }

                var messageType = _dispatcher.GetMessageTypeByName(messageTypeName)!;

                var messageAsType = (IMessage)JsonSerializer.Deserialize(message.Body, messageType)!;

                await _dispatcher.DispatchAsync(messageAsType);
                await _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, ct);
            }
        }
    }
}