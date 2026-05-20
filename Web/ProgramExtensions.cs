namespace Web
{
    public static class ProgramExtensions
    {
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
                    "css/stadiumDetails.css"
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
