
namespace RestAPIBackendWebService.Domain.Role.Constants
{
    public static class RolesHierarchy
    {
        private static readonly Dictionary<int, string> _rolesHierarchy = new Dictionary<int, string>
        {
            { 1, "User" },
            { 2, "Admin" }
        };
        public static Dictionary<int, string> RolesHierarchyDictionary { get => _rolesHierarchy; }

        public static KeyValuePair<int, string> GetHighestRoleInHierarchy(List<string> userRoles)
        {
            return _rolesHierarchy
                .Where(r => userRoles.Any(x => x.Equals(r.Value)))
                .OrderBy(r => r.Key)
                .FirstOrDefault();
        }

        public static KeyValuePair<int, string> GetHierarchyRole(string role)
        {
            return _rolesHierarchy.FirstOrDefault(r => r.Value.Equals(role));
        }

        public static List<string> GetRoleChildsInHierarchy(KeyValuePair<int, string> role)
        {
            return _rolesHierarchy.Where(r => r.Key > role.Key).Select(r => r.Value).ToList();
        }
        public static List<string> GetAllRoles()
        {
            return _rolesHierarchy.Values.ToList();
        }
        public static bool HasSuperAdminRole(List<string> userRoles)
        {
            var highestRoleInHierarchy = GetHighestRoleInHierarchy(userRoles);

            return highestRoleInHierarchy.Key == 2;
        }
    }
}
