using Aggregator.Infrastructure.Policies;
using Aggregator.Infrastructure.Services;
using Aggregator.WebApi.Services.ProtoServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Aggregator.Application.Contracts.Interfaces;
using Aggregator.Infrastructure.Services.ProtoServices;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GrpcGetUserChatsService>();
builder.Services.AddScoped<GrpcGetUserForChatService>();
builder.Services.AddScoped<GrpcGetUserForPostsService>();
builder.Services.AddScoped<GrpcGetUserPostsService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPostService, PostService>();
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
});
builder.Services.AddSingleton<GrpcPolly>();

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

        options.Audience = "Aggregator.WebApi";

    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
