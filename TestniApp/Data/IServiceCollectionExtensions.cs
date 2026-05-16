using Data.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:SqlLiteConnection"];

            services.AddDbContext<TicketDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddStores();
            services.AddHttpContextAccessor();
            services.AddScoped<AuthService>();


            return services;
        }

        private static IServiceCollection AddStores(this IServiceCollection services)
        {
            services.AddScoped<UserStore>();
            services.AddScoped<CategoryStore>();
            return services;
        }

    }
}
