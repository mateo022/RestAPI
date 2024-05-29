using RestAPIBackendWebService.Domain.User.Models;

namespace RestAPIBackendWebService.Domain.User.DTOs
{
    public class BasicUserInformationResponseDTO : BasicUserInformation
    {
        public string PhoneNumber { get; set; }
        public string PhoneNumberIndicator { get; set; }
    }
}
