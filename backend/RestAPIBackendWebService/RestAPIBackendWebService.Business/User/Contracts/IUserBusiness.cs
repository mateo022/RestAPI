using Microsoft.AspNetCore.Identity;
using RestAPIBackendWebService.Domain.Auth.Entities;
using RestAPIBackendWebService.Domain.Auth.Models;
using RestAPIBackendWebService.Domain.User.DTOs;
using RestAPIBackendWebService.Domain.User.Models;
using System.Security.Claims;


namespace RestAPIBackendWebService.Business.User.Contracts
{
    public interface IUserBusiness
    {
        public Task<List<BasicUserInformation>> GetAllUsersInformation(string requesterEmail);
        public CustomIdentityUser GetUserByEmailNotConfirmedAsync(string email);
        public Task<IdentityResult> InitialRegisterAsync(NewUserRequestDTO newUserData, ClaimsIdentity userClaimsIdentity);
        public Task<UserConfirmationResult> ConfirmNewUserInformationAsync(CompleteUserRegisterRequestDTO newUserData);
        public Task<CustomIdentityUser> GetUserInformationForEditByEmail(string email);
        public Task<EditUserResult> EditUserInformation(EditUserInformationRequestDTO userNewInfo);
        public Task<UserDeleteResult> DeleteUserByEmail(string userToDeleteEmail, string requesterEmail, List<string> requesterRoles);
        public Task<IdentityResult> RegisterAndConfirmUserAsync(NewUserRequestDTO newUserData, ClaimsIdentity requesterUserClaims);
    }
}
