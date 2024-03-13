using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot().AddPolly();

builder.Services.AddCors();

var app = builder.Build();
app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});
await app.UseOcelot();




app.Run();
