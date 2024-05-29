
using Microsoft.AspNetCore.Identity;


namespace RestAPIBackendWebService.Domain.Auth.Entities
{
    public class CustomIdentityUser : IdentityUser
    {
        public string PhoneNumberIndicator { get; set; }
        public string TwoFactorCode { get; set; }
        public DateTimeOffset TwoFactorExpireDate { get; set; }

    }
}
