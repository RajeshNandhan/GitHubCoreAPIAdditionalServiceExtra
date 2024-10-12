using Microsoft.Extensions.Configuration;
using System;

namespace Core.API.AdditionalService
{
    public static class ConnectionStringExtensions
    {
        /// <summary>
        /// NOT USED
        /// </summary>
        /// <param name="Configuration"></param>
        /// <returns></returns>
        public static IConfiguration GetConfigurationBasedOnAppsettingFile(this ConfigurationBuilder Configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(environment))
                environment = "Development";

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

            return config;
        }

        public static int IsMongoDB(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            switch (currentConfiguration.DBContext)
            {
                case "MongoDBContext":
                    return 1;
                default:
                    return 0;
            }
        }

        public static bool IsCacheRequired(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.Caching == "Empty" ? false : true;
        }

        public static string CurrentDBContext(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.DBContext;
        }

        public static string CurrentMessageType(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.Messaging;
        }

        public static string CurrenCachingType(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.Caching;
        }
        public static string CurrenApplicationInsightsType(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.ApplicationInsights;
        }

        public static string CurrenIdentityServerType(this IConfiguration configuration)
        {
            var currentConfigurationOption = configuration.GetSection("CurrentConfiguration");
            var currentConfiguration = currentConfigurationOption.Get<CurrentConfiguration>();

            return currentConfiguration.IdentityServer;
        }
    }
}