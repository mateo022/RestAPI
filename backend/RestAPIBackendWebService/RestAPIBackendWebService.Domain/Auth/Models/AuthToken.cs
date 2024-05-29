
using RestAPIBackendWebService.Domain.Common.Models;

namespace RestAPIBackendWebService.Business.Auth.Logic
{
    public class AuthToken : BaseResult
    {
        public string Username { get; set; }
        public string TokenValue { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
        public AuthToken() : base() { }

    }
}
