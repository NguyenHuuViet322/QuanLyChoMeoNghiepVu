namespace DAS.Web.Configuration
{
    public class AuthServerConfiguration
    {
        public string IdentityServerBaseUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public bool CorsAllowAnyOrigin { get; set; }

        public string[] CorsAllowOrigins { get; set; }

        public string[] ClientScopes { get; set; }
    }
}
