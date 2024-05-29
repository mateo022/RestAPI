using RestAPIBackendWebService.Domain.Common.Models;
using RestAPIBackendWebService.Domain.User.Models;


namespace RestAPIBackendWebService.Domain.Auth.Models
{
    public class LoginResultAuth : BaseResult
    {
        public AuthTokenModel Token { get; set; }
        public BasicUserInformation UserInformation { get; set; }
        public LoginResultAuth() : base() { }
    }
}
