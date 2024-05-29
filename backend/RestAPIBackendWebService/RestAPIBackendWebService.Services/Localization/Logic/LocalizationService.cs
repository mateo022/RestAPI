using RestAPIBackendWebService.Services.Localization.Contract;
using Newtonsoft.Json;

namespace RestAPIBackendWebService.Services.Localization.Logic
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILanguageProviderStrategy _languageProviderStrategy;
        

        public LocalizationService(ILanguageProviderStrategy languageProviderStrategy)
        {
            _languageProviderStrategy = languageProviderStrategy;
        }

        public Dictionary<string, string> GetTranslationsByFileName(string jsonFileName)
        {
            var jsonTextContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Localization\Translations\{jsonFileName}"));
            var allLanguagesTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonTextContent);

            return allLanguagesTranslations[_languageProviderStrategy.GetApplicationDefaultIsoLanguageCode()];
        }
    }
}
