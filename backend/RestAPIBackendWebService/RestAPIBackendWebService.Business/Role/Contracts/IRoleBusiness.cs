
using System.Security.Claims;

namespace RestAPIBackendWebService.Business.Role.Contracts
{
    public interface IRoleBusiness
    {
        public bool CanAssignRole(string newUserRole, List<string> requesterUserRoles);
        public List<string> GetValidRolesForUserCreate(ClaimsIdentity userClaimsIdentity);
        public List<string> GetUserRolesByClaims(ClaimsIdentity userClaimsIdentity);
    }
}
