using RestAPIBackendWebService.Services.Localization.Contract;

namespace RestAPIBackendWebService.Services.Localization.Logic
{
    public class LanguageProviderStrategy : ILanguageProviderStrategy
    {
        public string GetApplicationDefaultIsoLanguageCode()
        {
            /*
             * Initially the application will supports spanish.
             * In case of more languages added modify this method.
             */

            return "es";
        }
    }
}
