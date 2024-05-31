using RestAPIBackendWebService.Business.Role.Contracts;
using RestAPIBackendWebService.Domain.Role.Constants;

//using O2CBackendWebService.Domain.Role.Constants;
using System.Security.Claims;

namespace O2CBackendWebService.Business.Role.Logic
{
    public class RoleBusiness : IRoleBusiness
    {
        public bool CanAssignRole(string newUserRole, List<string> requesterUserRoles)
        {
            //If user is SuperAdmin return true
            if (requesterUserRoles.Contains(RolesHierarchy.RolesHierarchyDictionary[2]))
            {
                return true;
            }

            var userHighestRoleHierarchy = RolesHierarchy.GetHighestRoleInHierarchy(requesterUserRoles);
            var newUserRoleWithHierarchy = RolesHierarchy.GetHierarchyRole(newUserRole);

            if (userHighestRoleHierarchy.Value != null && newUserRoleWithHierarchy.Value != null)
            {
                if (userHighestRoleHierarchy.Key < newUserRoleWithHierarchy.Key)
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetValidRolesForUserCreate(ClaimsIdentity userClaimsIdentity)
        {
            var requesterUserRoles = GetUserRolesByClaims(userClaimsIdentity);
            var highestUserRoleInHierarchy = RolesHierarchy.GetHighestRoleInHierarchy(requesterUserRoles);
            //var result = RolesHierarchy.GetRoleChildsInHierarchy(highestUserRoleInHierarchy);
            var result = RolesHierarchy.GetAllRoles();

            // Imprimir el contenido de result
            Console.WriteLine("Contenido de result:");
            foreach (var role in result)
            {
                Console.WriteLine(role);
            }

            return result;
        }

        public List<string> GetUserRolesByClaims(ClaimsIdentity userClaimsIdentity)
        {
            return userClaimsIdentity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}
