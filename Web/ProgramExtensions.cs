using Data.Data;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Identity;

namespace Web
{
    public static class ProgramExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<MatchTrackerDbContext>();

            return services;
        }

        public static IServiceCollection AddCurrentUserServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<CurrentUserService>();
            return services;
        }
        public static IServiceCollection AddGoogleAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });

            return services;
        }

        public static IServiceCollection AddLiveMessaggingSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

            return services;
        }

        public static IServiceCollection BundleAndMinify(this IServiceCollection services)
        {
            services.AddWebOptimizer(pipeline =>
            {

                pipeline.AddCssBundle("~/css/all.min.css",
                    "css/dashboard.css",
                    "css/detailsShared.css",
                    "css/home.css",
                    "css/matchCard.css",
                    "css/matchDetails.css",
                    "css/partyCard.css",
                    "css/partyDetails.css",
                    "css/playerDetails.css",
                    "css/site.css",
                    "css/stadiumCard.css",
                    "css/stadiumDetails.css",
                    "css/userDashboard.css"
                    );

                pipeline.AddJavaScriptBundle("~/js/players.min.js", "js/players.js");
                pipeline.AddJavaScriptBundle("~/js/matchRecords.min.js", "js/matchRecords.js");
                pipeline.AddJavaScriptBundle("~/js/parties.min.js", "js/parties.js");
                pipeline.AddJavaScriptBundle("~/js/playingFields.min.js", "js/playingFields.js");

            });

            return services;
        }
    }
}
