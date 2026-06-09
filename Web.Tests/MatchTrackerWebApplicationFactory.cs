using Data.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Tests
{
    public class MatchTrackerWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var testConfig = new Dictionary<string, string?>
                {
                    ["Authentication:Google:ClientId"] = "test-client-id",
                    ["Authentication:Google:ClientSecret"] = "test-client-secret",
                };
                config.AddInMemoryCollection(testConfig);
            });

            builder.ConfigureTestServices(services =>  // ← was ConfigureServices
            {
                // Remove ALL descriptors related to the real DbContext
                var toRemove = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<MatchTrackerDbContext>) ||
                        d.ServiceType == typeof(MatchTrackerDbContext))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                // Register fresh InMemory DbContext
                services.AddDbContext<MatchTrackerDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));  // ← fixed name, no Guid

                // Make cookie auth return 401 instead of redirecting to login page,
                // so unauthenticated API calls get the expected status code in tests.
                services.ConfigureApplicationCookie(options =>
                {
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx =>
                        {
                            ctx.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = ctx =>
                        {
                            ctx.Response.StatusCode = 403;
                            return Task.CompletedTask;
                        },
                    };
                });
            });
        }
    }
}
