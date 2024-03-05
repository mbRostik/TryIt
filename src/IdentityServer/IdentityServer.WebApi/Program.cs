using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer.Infrastructure.Data;
using IdentityServer.WebApi;
using MassTransit;
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

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((cxt, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(cxt);
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();


app.MapControllers();

//SeedData.EnsureSeedData(app);

app.Run();

