using Chats.Application.Contracts.Interfaces;
using Chats.Application.UseCases.Consumers;
using Chats.Application.UseCases.Queries;
using Chats.Infrastructure.Data;
using Chats.Infrastructure.Services;
using Chats.Infrastructure.Services.grpcServices;
using Chats.WebApi.ChatHubSpace;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using FluentValidation.AspNetCore;
using Chats.Application.Validators;
using MessageBus.Messages.Events.IdentityServerService;
using System.Net;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Server.Kestrel.Core;


var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    options.Listen(IPAddress.Any, 8082, listenOptions =>
    {
        listenOptions.UseHttps("https/chatwebapi-api.pfx", "pa55w0rd!");
    });

});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddGrpc();

builder.Services.AddScoped<IMapperService, MapperService>();
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GetChatIdDTOValidator>());


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
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateLifetime = false;
        options.TokenValidationParameters.RequireExpirationTime = false;
        options.TokenValidationParameters.RequireSignedTokens = false;
        options.TokenValidationParameters.RequireAudience = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;

        options.TokenValidationParameters.SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            var jwtHandler = new JsonWebTokenHandler();
            var jsonToken = jwtHandler.ReadJsonWebToken(token);
            return jsonToken;
        };
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TempData"));
        var jwtBearerSettings = builder.Configuration.GetSection("JwtBearer");

        options.Authority = jwtBearerSettings["Authority"];

        options.Audience = "Chats.WebApi";
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken2 = context.Request.Query["access_token"];
                Console.WriteLine(accessToken2 + "\n\n\n\n");
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken2) &&
                    (path.StartsWithSegments("/SendMessage")))
                {
                    context.Token = accessToken2;
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
        options.MaximumReceiveMessageSize = 10000000;
        options.EnableDetailedErrors = true;
        options.KeepAliveInterval = TimeSpan.FromMinutes(5);
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
        cfg.Publish<IUserCreate_SendEvent_From_ChatWebApi>(p => p.ExchangeType = ExchangeType.Fanout);

        cfg.ReceiveEndpoint("rabbitChatWebApiQueue", e =>
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
    var context = services.GetRequiredService<ChatDbContext>();
    context.Database.Migrate();
}
app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:5173")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
});

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
