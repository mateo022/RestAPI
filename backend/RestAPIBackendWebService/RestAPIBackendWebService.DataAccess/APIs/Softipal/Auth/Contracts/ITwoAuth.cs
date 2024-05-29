
using RestAPIBackendWebService.Domain.Auth.DTOs;

namespace RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Contracts
{
    public interface ITwoAuth
    {
        public Task<bool> TwoFactorCode(TwoAuthRequestDTO requestBody);
    }
}
