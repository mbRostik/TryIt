using MassTransit;
using SagasStateMachines.WebApi.StateMachines.PostStateMachines;
using SagasStateMachines.WebApi.StateMachines.UserStateMachines;
using SagasStateMachines.WebApi.States.PostStates;
using SagasStateMachines.WebApi.States.UserStates;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/sagawebapi-api.pfx", "pa55w0rd!");
    });
});


builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
        {
            IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
            AutoRegisterTemplate = true,
            NumberOfReplicas = 1,
            NumberOfShards = 2
        })
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .ReadFrom.Configuration(context.Configuration);
});


builder.Services.AddMassTransit(x =>
{

    // потім в appsettings закину
    x.AddSagaStateMachine<UserCreationStateMachine, ProcessingUserCreationState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://root:example@mongo:27017";
            r.DatabaseName = "UserCreation_Saga";
        });

    x.AddSagaStateMachine<PostCreationStateMachine, ProcessingPostCreationState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://root:example@mongo:27017";
            r.DatabaseName = "PostCreation_Saga";
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        var inputUserQueue = "sagas-usercreation-processor";
        cfg.ReceiveEndpoint(inputUserQueue, e =>
        {
            e.ConfigureSaga<ProcessingUserCreationState>(context);
        });

        var inputPostQueue = "sagas-postcreation-processor";
        cfg.ReceiveEndpoint(inputPostQueue, e =>
        {
            e.ConfigureSaga<ProcessingPostCreationState>(context);
        });
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
