using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Data.Data;

namespace Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MatchTrackerSqlite");
            services.AddDbContext<MatchTrackerDbContext>(options => options.UseSqlite(connectionString));

            return services;
        }
    }
}
