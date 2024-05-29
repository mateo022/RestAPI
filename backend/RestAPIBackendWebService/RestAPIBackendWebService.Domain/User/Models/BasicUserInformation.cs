namespace RestAPIBackendWebService.Domain.User.Models
{
    public class BasicUserInformation
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}
