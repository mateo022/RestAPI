using RestAPIBackendWebService.Services.Security.Logic;

namespace RestAPIBackendWebService.Extensions
{
    public static class ServicesExtension
    {
        public const string DEVELOPMENT_CORS = "_developmentCorsPolicy";

        public static void RegisterDependencies(this IServiceCollection services)
        {
            #region SERVICES LAYER
            services.AddSingleton<IJwtService, JwtService>();
            #endregion


            #region BUSINESS LAYER
            #endregion

            #region DATA ACCESS LAYER
            #endregion
        }
    }
}