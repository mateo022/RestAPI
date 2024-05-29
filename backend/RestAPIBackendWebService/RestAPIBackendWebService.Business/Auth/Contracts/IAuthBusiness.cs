using RestAPIBackendWebService.Business.Auth.Logic;
using RestAPIBackendWebService.Domain.Auth.Models;
using RestAPIBackendWebService.Domain.Services.Mailing;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Domain.Auth.Models;
using RestAPIBackendWebService.Domain.User.DTOs;



namespace RestAPIBackendWebService.Business.Auth.Contracts
{
    public interface IAuthBusiness
    {
        public Task<LoginResultAuth> Login(LoginRequestDTO request);
        public Task LogoutAsync(string userEmail);
        public Task<SendMailResult> ForgotPasswordEmailGenerationAsync(UserEmailDTO userEmail);
        public Task<LoginResultAuth> VerifyTwoFactorCode(string phoneNumber, string code);

    }
}
