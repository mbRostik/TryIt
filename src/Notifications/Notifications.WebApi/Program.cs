using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using Microsoft.EntityFrameworkCore;
using Notifications.Application.UseCases.Consumers;
using Notifications.Application.UseCases.Queries;
using Notifications.Infrastructure.Data;
using RabbitMQ.Client;
using System.Net;

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
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/notificationwebapi-api.pfx", "pa55w0rd!");
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

        cfg.Publish<IUserCreate_SendEvent_From_NotificationWebApi>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.ReceiveEndpoint("rabbitNotificationWebApiQueue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(context);
        });
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NotificationDbContext>();
    context.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
