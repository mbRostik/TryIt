using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Polly;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/ocelotgateway-api.pfx", "pa55w0rd!");
    });
});
builder.Services.AddHttpClient("userwebapi-api", c =>
{
    c.BaseAddress = new Uri("https://userwebapi-api:8081");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot().AddPolly();

builder.Services.AddCors();

var app = builder.Build();
app.UseCors(policyBuilder =>
{
    policyBuilder.WithOrigins("https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
await app.UseOcelot();




app.Run();
