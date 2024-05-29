using RestAPIBackendWebService.Domain.Services.Mailing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPIBackendWebService.Business.Mailer.Contracts
{
    public interface IMailerBusiness
    {
        public Task<SendMailResult> SendRegisterEmailAsync(AuthEmail emailData);
        public Task<SendMailResult> SendPasswordRecoveryEmailAsync(AuthEmail emailData);
    }
}
