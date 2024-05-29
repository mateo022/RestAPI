
using RestAPIBackendWebService.Domain.Common.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
      public class NewUserRequestDTO
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
