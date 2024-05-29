using RestAPIBackendWebService.Domain.Services.Mailing;

namespace RestAPIBackendWebService.Services.Mailing.Contract
{
    public interface IMailerService
    {
        public Task<SendMailResult> SendEmailAsync(Email email);
        public string GetTemplate(string templatePath, Dictionary<string, string> templateData);
    }
}
