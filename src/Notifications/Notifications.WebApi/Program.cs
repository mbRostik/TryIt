using MassTransit;
using MessageBus.Messages.IdentityServerService;
using Microsoft.EntityFrameworkCore;
using Notifications.Application.UseCases.Consumers;
using Notifications.Application.UseCases.Queries;
using Notifications.Infrastructure.Data;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.Services.AddDbContext<NotificationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(GetAllNotificationsQuery).Assembly);

});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<IdentityUserCreatedEvent>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.ReceiveEndpoint("notifications_UserConsumer_queue", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
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
