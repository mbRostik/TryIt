using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer.Infrastructure.Data;
using IdentityServer.WebApi;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using MessageBus.Messages.Events.IdentityServerService;
using RabbitMQ.Client;
using IdentityServer.Application.UseCases.Consumers;
using System.Net;
var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var assembly2 = typeof(IdentityServerDbContext).Assembly.GetName().Name;

var defaultConnString = builder.Configuration.GetConnectionString("MSSQLConnection");
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 8080);
    options.Listen(IPAddress.Any, 8081, listenOptions =>
    {
        listenOptions.UseHttps("https/identityserverapi-api.pfx", "pa55w0rd!");
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("uk-UA"),
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedUICultures = supportedCultures;
    options.SupportedCultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider()
    {
        CookieName = CookieRequestCultureProvider.DefaultCookieName
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


builder.Services.AddDbContext<IdentityServerDbContext>(options =>
{
    options.UseSqlServer(defaultConnString);
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { options.SignIn.RequireConfirmedAccount = true; options.User.AllowedUserNameCharacters += " "; })
    .AddEntityFrameworkStores<IdentityServerDbContext>()
     .AddDefaultTokenProviders();



builder.Services.AddDataProtection()
    .SetApplicationName("IdentityServer.WebApi")
    .PersistKeysToDbContext<IdentityServerDbContext>();


builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
})
    .AddConfigurationStore<ConfigurationDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString,
            sql => sql.MigrationsAssembly(assembly2));
    })
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString,
            sql => sql.MigrationsAssembly(assembly2));
    })
    .AddAspNetIdentity<IdentityUser>()
    .AddDeveloperSigningCredential();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreation_Consumer>();

    x.UsingRabbitMq((cxt, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Publish<IUserCreate_SendEvent_From_IdentityServer>(p => p.ExchangeType = ExchangeType.Fanout);
        cfg.ReceiveEndpoint("users_UserProcessedConsumer_queue", e =>
        {
            e.ConfigureConsumer<UserCreation_Consumer>(cxt);
        });
    });

});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.SignInScheme = Duende.IdentityServer.IdentityServerConstants.ExternalCookieAuthenticationScheme;

        var googleAuth = builder.Configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuth["ClientId"];
        options.ClientSecret = googleAuth["ClientSecret"];

    });
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}


using (var scope = app.Services.CreateScope())
{
    try
    {
        var identityServerDb = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
        var configDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        var persistedGrantDb = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        configDb.Database.Migrate();
        persistedGrantDb.Database.Migrate();
        identityServerDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Smth went wrong");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();
app.UseIdentityServer();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

SeedData.EnsureSeedData(app);

app.Run();

