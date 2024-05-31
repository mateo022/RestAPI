
using RestAPIBackendWebService.Domain.Common.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
      public class NewUserRequestDTO: UserEditBase
    {
        [LocalizationRequired]
        [LocalizationEmailAddress]
        public string Email { get; set; }

        [LocalizationRequired]
        public string Name { get; set; }

        [LocalizationRequired]
        public string RoleName { get; set; }
    }
}
