using RestAPIBackendWebService.Domain.Common.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
    public class UserEditBase : UserBase
    {
        [LocalizationRequired]
        [LocalizationPhone]
        public string PhoneNumber { get; set; }

        [LocalizationRequired]
        [LocalizationPhoneIndicator]
        public string PhoneNumberIndicator { get; set; }

        [LocalizationRequired]
        [LocalizationCompare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
