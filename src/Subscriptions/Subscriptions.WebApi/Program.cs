using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Subscriptions.Application.UseCases.Consumers;
using Subscriptions.Application.UseCases.Queries;
using Subscriptions.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.Services.AddDbContext<SubscriptionDbContext>(options =>
{
    options.UseSqlServer(connectionString);
}); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(GetAllSubscriptionsQuery).Assembly);

});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<IUserCreate_SendEvent_From_SubscriptionWebApi>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.ReceiveEndpoint("rabbitSubscriptionWebApiQueue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(context);
        });
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
