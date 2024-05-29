using RestAPIBackendWebService.Domain.Common.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
    public class UserBase
    {
        [LocalizationRequired]
        [LocalizationEmailAddress]
        public string Email { get; set; }

        [LocalizationRequired]
        public string Password { get; set; }
    }
}
