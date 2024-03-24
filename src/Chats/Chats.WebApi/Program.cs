using Chats.Application.Contracts.Interfaces;
using Chats.Application.UseCases.Consumers;
using Chats.Application.UseCases.Queries;
using Chats.Infrastructure.Data;
using Chats.Infrastructure.Services;
using Chats.Infrastructure.Services.grpcServices;
using Chats.WebApi.ChatHubSpace;
using MassTransit;
using MessageBus.Messages.IdentityServerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddScoped<IMapperService, MapperService>();


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



builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(GetAllChatsQuery).Assembly);

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtBearerSettings = builder.Configuration.GetSection("JwtBearer");

        options.Authority = jwtBearerSettings["Authority"];

        options.Audience = "Chats.WebApi";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/SendMessage")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
        };
    });

builder.Services
    .AddSignalR(options =>
    {
        options.HandshakeTimeout = TimeSpan.FromHours(1);
    })
    .AddHubOptions<ChatHub>(options =>
{
    options.MaximumReceiveMessageSize= 10000000;
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromMinutes(5);
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
        cfg.ReceiveEndpoint("chats_UserConsumer_queue", e =>
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

app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:5173") 
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials(); 
});

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseRouting(); 

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/SendMessage");

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<grpcUserChats_Service>();
    endpoints.MapControllers(); 

    endpoints.MapGet("../Chats.Application/Contracts/protos/userchats.proto", async context =>
    {
        var protoPath = Path.Combine(app.Environment.ContentRootPath, "../Chats.Application/Contracts/protos/userchats.proto");
        await context.Response.WriteAsync(await File.ReadAllTextAsync(protoPath));
    });
});

app.Run();
