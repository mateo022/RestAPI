namespace RestAPIBackendWebService.Services.Localization.Contract
{
    public interface ILanguageProviderStrategy
    {
        /*
         * Return ISO 639-1 code of default language for application
         */
        public string GetApplicationDefaultIsoLanguageCode();
    }
}
