namespace RestAPIBackendWebService.Extensions
{
    public static class ServicesExtension
    {
        public const string DEVELOPMENT_CORS = "_developmentCorsPolicy";

        public static void RegisterDependencies(this IServiceCollection services)
        {
            #region SERVICES LAYER

            services.AddSingleton<IJwtService, JwtService>();
   
        }
}
