namespace Web
{
    public static class ProgramExtensions
    {
        public static IServiceCollection BundleAndMinify(this IServiceCollection services)
        {
            services.AddWebOptimizer(pipeline =>
            {

                pipeline.AddCssBundle("~/css/all.min.css",
                    "css/detailsShared.css",
                    "css/home.css",
                    "css/matchCard.css",
                    "css/matchDetails.css",
                    "css/partyCard.css",
                    "css/partyDetails.css",
                    "css/playerDetails.css",
                    "css/site.css",
                    "css/stadiumCard.css",
                    "css/stadiumDetails.css"
                    );

            });

            return services;
        }
    }
}
