using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Data.Data;
using Data.Models;
using Data.Services.Stores;
using Data.Services.Validation;
using Data.Services.Validation.Interfaces;

namespace Data
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MatchTrackerSqlite");
            services.AddDbContext<MatchTrackerDbContext>(options => options.UseSqlite(connectionString));

            services.AddStores();
            services.AddValidators();

            return services;
        }

        private static IServiceCollection AddStores(this IServiceCollection services)
            => services
                .AddScoped<MatchStore>()
                .AddScoped<MatchPlayerStore>()
                .AddScoped<MatchVoteStore>()
                .AddScoped<PartyStore>()
                .AddScoped<PlayerStore>()
                .AddScoped<PlayerRatingStore>()
                .AddScoped<PreferredPlayingDateStore>()
                .AddScoped<ScheduledMatchStore>()
                .AddScoped<ScheduledMatchAttendanceStore>()
                .AddScoped<StadiumStore>();

        private static IServiceCollection AddValidators(this IServiceCollection services)
            => services
                .AddScoped<IValidator<MatchPlayer>, MatchPlayerValidator>()
                .AddScoped<IValidator<MatchRecord>, MatchRecordValidator>()
                .AddScoped<IValidator<MatchVote>, MatchVoteValidator>()
                .AddScoped<IValidator<Party>, PartyValidator>()
                .AddScoped<IValidator<Player>, PlayerValidator>()
                .AddScoped<IValidator<PlayerRating>, PlayerRatingValidator>()
                .AddScoped<IValidator<PlayingField>, PlayingFieldValidator>()
                .AddScoped<IValidator<PreferredPlayingDate>, PreferredPlayingDateValidator>()
                .AddScoped<IValidator<ScheduledMatch>, ScheduledMatchValidator>()
                .AddScoped<IValidator<ScheduledMatchAttendance>, ScheduledMatchAttendanceValidator>();
    }
}
