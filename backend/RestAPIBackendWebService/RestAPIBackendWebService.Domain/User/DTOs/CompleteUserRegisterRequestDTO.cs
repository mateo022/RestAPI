using RestAPIBackendWebService.Domain.Common.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
    public class CompleteUserRegisterRequestDTO : UserEditBase
    {
        [LocalizationRequired]
        public string Token { get; set; }
    }
}
