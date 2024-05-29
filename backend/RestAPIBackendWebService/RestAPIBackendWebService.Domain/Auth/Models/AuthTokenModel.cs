using RestAPIBackendWebService.Domain.Common.Models;

namespace RestAPIBackendWebService.Domain.Auth.Models
{
    public class AuthTokenModel: BaseResult
    {
        public string TokenValue { get; set; }
        public DateTime Expiration { get; set; }
    }
}

