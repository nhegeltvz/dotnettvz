using Data.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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

                // Register a fake auth scheme so tests can simulate authenticated users
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme    = TestAuthHandler.SchemeName;
                })
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

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

        /// <summary>
        /// Sets the authenticated user the test auth handler will impersonate.
        /// Pass the AppUser.Id — the controller resolves the Player via UserManager.GetUserId().
        /// Call with null to simulate an unauthenticated request.
        /// </summary>
        public void SetAuthenticatedUser(Guid? userId)
        {
            var options = Services.GetRequiredService<IOptionsMonitor<TestAuthHandlerOptions>>()
                                  .Get(TestAuthHandler.SchemeName);
            options.Claims = userId.HasValue
                ? [new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())]
                : [];
        }

        public void SetAuthenticatedAdmin(Guid? userId = null)
        {
            var options = Services.GetRequiredService<IOptionsMonitor<TestAuthHandlerOptions>>()
                                  .Get(TestAuthHandler.SchemeName);
            var id = userId ?? Guid.NewGuid();
            options.Claims =
            [
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
            ];
        }
    }
}
