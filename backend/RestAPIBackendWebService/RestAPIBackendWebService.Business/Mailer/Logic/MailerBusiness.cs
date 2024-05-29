
using RestAPIBackendWebService.Domain.Services.Mailing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestAPIBackendWebService.Business.Mailer.Contracts;
using RestAPIBackendWebService.DataAccess;
using RestAPIBackendWebService.Domain.Auth.Entities;
using RestAPIBackendWebService.Domain.Services.Localization;
using System.Web;
using RestAPIBackendWebService.Services.Mailing.Contract;

namespace RestAPIBackendWebService.Business.Mailer.Logic
{
    public class MailerBusiness : IMailerBusiness
    {
        private readonly IMailerService _mailerService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly RestAPIDbContext _dbContext;

        private const string COMPLETE_REGISTER_EMAIL_TEMPLATE_NAME = "CompleteRegister.html";
        private const string RECOVERY_PASSWORD_EMAIL_TEMPLATE_NAME = "RecoveryPassword.html";
        private const string COMPLETE_REGISTER_SUBJECT = "Boston Scientific - Completar Registro";
        private const string RECOVERY_PASSWORD_SUBJECT = "Boston Scientific - Recuperar contraseña";

        public MailerBusiness(IMailerService mailerService,
            IConfiguration configuration,
            UserManager<CustomIdentityUser> userManager,
            RestAPIDbContext dbContext)
        {
            _mailerService = mailerService;
            _configuration = configuration;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<SendMailResult> SendRegisterEmailAsync(AuthEmail emailData)
        {
            var user = await _userManager.FindByEmailAsync(emailData.Email);
         

            if (user == null)
            {
                return new SendMailResult
                {
                    Message = String.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], emailData.Email),
                    Success = false
                };
            }
         
            if (user.EmailConfirmed)
            {
                return new SendMailResult
                {
                    Message = String.Format(ApplicationTranslations.IdentityErrors["DuplicateEmail"], emailData.Email),
                    Success = false
                };
            }

            var templateData = new Dictionary<string, string>()
            {
                {
                    "uxServer",
                    _configuration["UxServer"]
                },
                {
                    "ConfirmationLink",
                    String.Format(_configuration["CompleteRegisterUrl"], HttpUtility.UrlEncode(emailData.Email), HttpUtility.UrlEncode(emailData.Token))
                },
                {
                    "userName",
                    user.UserName
                },
                {
                    "userEmail",
                    emailData.Email
                }
            };

            var template = _mailerService.GetTemplate(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Mailer\MailTemplates\{COMPLETE_REGISTER_EMAIL_TEMPLATE_NAME}"),
                templateData);

            var mailingResult = await _mailerService.SendEmailAsync(new Email
            {
                From = _configuration["EmailFrom"],
                FromName = _configuration["EmailFromName"],
                To = emailData.Email,
                Subject = COMPLETE_REGISTER_SUBJECT,
                Body = template,
                EnableSsl = true,
                Host = _configuration["EmailHost"],
                Port = Convert.ToInt32(_configuration["EmailPort"]),
                SmtpUserName = _configuration["EmailSmtpUserName"],
                SmtpPassword = _configuration["EmailSmtpPassword"]
            });

            return mailingResult;
        }

        public async Task<SendMailResult> SendPasswordRecoveryEmailAsync(AuthEmail emailData)
        {
            var user = await _userManager.FindByEmailAsync(emailData.Email);

            if (user == null)
            {
                return new SendMailResult
                {
                    Message = String.Format(ApplicationTranslations.IdentityErrors["UserDontExists"], emailData.Email),
                    Success = false
                };
            }

            var templateData = new Dictionary<string, string>()
            {
                {
                    "uxServer",
                    _configuration["UxServer"]
                },
                {
                    "ConfirmationLink",
                    String.Format(_configuration["RecoveryPasswordUrl"], HttpUtility.UrlEncode(emailData.Email), HttpUtility.UrlEncode(emailData.Token))
                }
            };

            var template = _mailerService.GetTemplate(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Mailer\MailTemplates\{RECOVERY_PASSWORD_EMAIL_TEMPLATE_NAME}"),
                templateData);

            var mailingResult = await _mailerService.SendEmailAsync(new Email
            {
                From = _configuration["EmailFrom"],
                FromName = _configuration["EmailFromName"],
                To = emailData.Email,
                Subject = RECOVERY_PASSWORD_SUBJECT,
                Body = template,
                EnableSsl = true,
                Host = _configuration["EmailHost"],
                Port = Convert.ToInt32(_configuration["EmailPort"]),
                SmtpUserName = _configuration["EmailSmtpUserName"],
                SmtpPassword = _configuration["EmailSmtpPassword"]
            });

            return mailingResult;
        }
    }
}