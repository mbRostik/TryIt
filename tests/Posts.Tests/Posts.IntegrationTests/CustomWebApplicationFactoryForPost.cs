using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Posts.Infrastructure.Data;
using Posts.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Posts.IntegrationTests
{
    public class CustomWebApplicationFactoryForPost : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddControllers().AddApplicationPart(typeof(PostController).Assembly); 

                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ConnectionStrings:MSSQLConnection"] = "Server=(localdb)\\mssqllocaldb;Database=IntegrationTestsDb;Trusted_Connection=True;"
                    })
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
                services.AddDbContext<PostDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("MSSQLConnection")));
                services.AddSingleton<ILogger>(Mock.Of<ILogger>());
                services.AddSingleton<IMediator>(Mock.Of<IMediator>());
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            });
        }


        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock) { }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "TestUser") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}