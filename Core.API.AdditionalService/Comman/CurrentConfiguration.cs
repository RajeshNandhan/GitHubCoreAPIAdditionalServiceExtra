namespace Core.API.AdditionalService
{
    public class CurrentConfiguration
    {
        public string DBContext
        {
            get;
            set;
        }

        public string Messaging
        {
            get;
            set;
        }

        public string Caching
        {
            get;
            set;
        }
        public string ApplicationInsights
        {
            get;
            set;
        }

        public string IdentityServer
        {
            get;
            set;
        }
    }
}
