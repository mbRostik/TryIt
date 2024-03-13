using MassTransit;
using MessageBus.Messages.IdentityServerService;
using MessageBus.Messages.PostService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Users.Application.UseCases.Consumers;
using Users.Application.UseCases.Queries;
using Users.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.Services.AddDbContext<UserDbContext>(options =>
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
    x.AddConsumer<UserCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<PostCreatedEvent>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.Publish<IdentityUserCreatedEvent>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint("users_PostConsumer_queue", e =>
        {
            e.ConfigureConsumer<PostCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("users_UserConsumer_queue", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
        });
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7174";

        options.Audience = "Users.WebApi";

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
