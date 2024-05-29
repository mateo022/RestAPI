using RestAPIBackendWebService.Services.Mailing.Contract;
using System.Net.Mail;
using System.Net;
using RestAPIBackendWebService.Domain.Services.Mailing;
using RestAPIBackendWebService.Services.Logger.Contract;
using Newtonsoft.Json;

namespace BostonOrderDeliveriesManagementAPI.Services.Mailing.Logic
{
    public class MailerService : IMailerService
    {
        private readonly ILoggerService _logger;

        public MailerService(ILoggerService logger)
        {
            _logger = logger;
        }

        public Task<SendMailResult> SendEmailAsync(Email email)
        {
            return Task.Run(() => SendEmail(email));
        }
        private SendMailResult SendEmail(Email email)
        {
            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(email.From, email.FromName);
            message.To.Add(new MailAddress(email.To));
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.IsBodyHtml = true;
            
            if(email.ConfigSet != null)
                message.Headers.Add("X-SES-CONFIGURATION-SET", email.ConfigSet);

            var mailingResult = new SendMailResult();

            using (var client = new System.Net.Mail.SmtpClient(email.Host, email.Port))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(email.SmtpUserName, email.SmtpPassword);

                // Enable SSL encryption
                client.EnableSsl = email.EnableSsl;

                //Serilze mail info for log
                var serializedResultForLog = JsonConvert.SerializeObject(new { To = email.To, Subject = email.Subject });

                // Try to send the message. Show status in console.
                try
                {
                    _logger.LogInfo($"{DateTime.UtcNow} - Attempting to send email to {email.To}");
                    
                    client.Send(message);

                    mailingResult.Success = true;
                    mailingResult.Message = "Correo enviado.";

                    _logger.LogInfo($"{mailingResult.Message}: {serializedResultForLog}");
                }
                catch (Exception)
                {
                    mailingResult.Success = false;
                    mailingResult.Message = "Envío de correo fallido.";

                    _logger.LogError($"{mailingResult.Message}: {serializedResultForLog}");
                }
            }

            return mailingResult;
        }

        public string GetTemplate(string templatePath, Dictionary<string, string> templateData)
        {
            var fileContent = File.ReadAllText(templatePath);

            foreach(var item in templateData)
            {
                if(!String.IsNullOrEmpty(item.Key) && !String.IsNullOrEmpty(item.Value))
                {
                    fileContent = fileContent.Replace($"{{{{{item.Key}}}}}", item.Value);
                }
            }

            return fileContent;
        }
    }
}
