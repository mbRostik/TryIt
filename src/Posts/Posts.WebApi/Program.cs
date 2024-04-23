using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Consumers;
using Posts.Application.UseCases.Queries;
using Posts.Infrastructure.Data;
using Posts.Infrastructure.Services;
using Posts.Infrastructure.Services.grpcServices;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("MSSQLConnection");

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddGrpc();

builder.Services.AddScoped<IMapperService, MapperService>();
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


builder.Services.AddDbContext<PostDbContext>(options =>
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
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<IUserCreate_SendEvent_From_PostWebApi>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.ReceiveEndpoint("rabbitPostWebApiQueue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(context);
        });
    });
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

        options.Audience = "Posts.WebApi";

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
    var context = services.GetRequiredService<PostDbContext>();
    context.Database.Migrate();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<grpcUserPosts_Service>();
    endpoints.MapControllers();

    endpoints.MapGet("../Users.Application/Contracts/protos/userforchat.proto", async context =>
    {
        var protoPath = Path.Combine(app.Environment.ContentRootPath, "../Posts.Application/Contracts/protos/userposts.proto");
        await context.Response.WriteAsync(await File.ReadAllTextAsync(protoPath));
    });

});
app.Run();
