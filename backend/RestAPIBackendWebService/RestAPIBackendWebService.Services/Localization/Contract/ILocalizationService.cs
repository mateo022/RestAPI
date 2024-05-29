
namespace RestAPIBackendWebService.Services.Localization.Contract
{
    public interface ILocalizationService
    {
        public Dictionary<string, string> GetTranslationsByFileName(string jsonFileName);
    }
}
