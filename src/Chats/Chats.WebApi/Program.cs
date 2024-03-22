using Chats.Application.UseCases.Consumers;
using Chats.Application.UseCases.Queries;
using Chats.Infrastructure.Data;
using Chats.Infrastructure.Services.grpcServices;
using Chats.WebApi.ChatHubSpace;
using MassTransit;
using MessageBus.Messages.IdentityServerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");
builder.Services.AddGrpc();

builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblies(typeof(GetAllChatsQuery).Assembly);

});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7174";

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
builder.Services.AddCors();

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
