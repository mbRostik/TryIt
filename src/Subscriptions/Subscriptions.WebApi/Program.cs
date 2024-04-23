using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Subscriptions.Application.UseCases.Consumers;
using Subscriptions.Application.UseCases.Queries;
using Subscriptions.Infrastructure.Data;
using System.Net;

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
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/subscriptionwebapi-api.pfx", "pa55w0rd!");
    });
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SubscriptionDbContext>();
    context.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
