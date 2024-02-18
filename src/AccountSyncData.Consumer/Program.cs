using AccountSyncData.Consumer;
using AccountSyncData.Consumer.Handler;
using Amazon;
using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddHostedService<SqsConsumerService>();

// builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.APSoutheast2));

builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient("AKIA26SBV7LFWP3H3MOP", "7zfmAWXrpK6VNddpEde4B/xOlL4mwAeMc0syVII9", RegionEndpoint.APSoutheast2));

builder.Services.AddSingleton<MessageDispatcher>();

builder.Services.AddScoped<MemberJoinedHandler>();
builder.Services.AddScoped<MemberDetailChangedHandler>();
builder.Services.AddMessageHandlers();

var app = builder.Build();

app.Run();