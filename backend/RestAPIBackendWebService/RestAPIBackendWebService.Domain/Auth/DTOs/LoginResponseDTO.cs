using RestAPIBackendWebService.Business.Auth.Logic;
using RestAPIBackendWebService.Domain.User.Models;

namespace BostonOrderDeliveriesManagementAPI.Domain.Auth.DTOs
{
    public class LoginResponseDTO
    {
        public AuthToken AuthToken { get; set; }
        public BasicUserInfo UserInformation { get; set; }
    }
}
