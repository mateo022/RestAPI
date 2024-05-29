using RestAPIBackendWebService.Business.Auth.Logic;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using RestAPIBackendWebService.Domain.Auth.Models;

namespace RestAPIBackendWebService.Services.Security.Contracts
{
    public interface IJwtService
    {
        public AuthTokenModel BuildToken(LoginRequestDTO userCredentials, List<string> permissions);
    }
}
