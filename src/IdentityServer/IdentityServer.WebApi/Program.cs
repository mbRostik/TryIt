using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer.Infrastructure.Data;
using IdentityServer.WebApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("MSSQLConnection");


builder.Services.AddDbContext<IdentityServerDbContext>(options =>
{
    options.UseSqlServer(defaultConnString);
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityServerDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
            sql => sql.MigrationsAssembly(assembly));
    })
    .AddOperationalStore<PersistedGrantDbContext>(options =>
    {
        options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString,
            sql => sql.MigrationsAssembly(assembly));
    })
    .AddAspNetIdentity<IdentityUser>()
    .AddDeveloperSigningCredential();
    //.AddInMemoryApiResources(Config.ApiResources)
    //.AddInMemoryApiScopes(Config.ApiScopes)
    //.AddInMemoryClients(Config.Clients)
    //.AddInMemoryIdentityResources(Config.IdentityResources);



var app = builder.Build();
app.UseIdentityServer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SeedData.EnsureSeedData(app);

app.Run();

