
namespace RestAPIBackendWebService.Domain.Auth.Models
{
    public class AuthLogData
    {
        public string userEmail { get; set; }

    }

    public enum AuthLogType
    {
        LOGIN,
        LOGOUT
    }
}
