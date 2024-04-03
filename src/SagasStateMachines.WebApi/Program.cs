using MassTransit;
using SagasStateMachines.WebApi.StateMachines.UserStateMachines;
using SagasStateMachines.WebApi.States.UserStates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<UserCreationStateMachine, ProcessingUserCreationState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://localhost:27017";
            r.DatabaseName = "UserCreation_Saga";
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        var inputQueue = "sagas-usercreation-processor";
        cfg.ReceiveEndpoint(inputQueue, e =>
        {
            e.ConfigureSaga<ProcessingUserCreationState>(context);
        });
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
