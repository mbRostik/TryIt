using MassTransit;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.PostService;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Reports.Application.UseCases.Consumers;
using Reports.Application.UseCases.Queries;
using Reports.Infrastructure.Data;

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

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PostCreatedConsumer>();
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<PostCreatedEvent>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.Publish<IUserCreate_Send_To_ReportWebApi>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint("reports_PostConsumer_queue", e =>
        {
            e.ConfigureConsumer<PostCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("rabbitReportWebApiQueue", e =>
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
