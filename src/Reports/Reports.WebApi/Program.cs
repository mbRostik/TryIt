using MassTransit;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Messages.Events.PostService;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Reports.Application.UseCases.Consumers;
using Reports.Application.UseCases.Queries;
using Reports.Infrastructure.Data;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.Services.AddDbContext<ReportDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(GetAllPostsQuery).Assembly);

});
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/reportwebapi-api.pfx", "pa55w0rd!");
    });
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PostCreation_Consumer>();
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<IPostCreate_SendEvent_From_ReportWebApi>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.Publish<IUserCreate_SendEvent_From_ReportWebApi>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint("rabbitReportWebApiQueue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(context);
            e.ConfigureConsumer<PostCreation_Consumer>(context);

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
    var context = services.GetRequiredService<ReportDbContext>();
    context.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
