
namespace RestAPIBackendWebService.Domain.User.Models
{
    public class BasicUserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Cellphone { get; set; }
        public double Credits { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }
    }
}
