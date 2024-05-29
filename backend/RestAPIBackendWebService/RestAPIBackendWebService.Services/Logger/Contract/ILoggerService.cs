
using RestAPIBackendWebService.Domain.Auth.Models;

namespace RestAPIBackendWebService.Services.Logger.Contract
{
    public interface ILoggerService
    {
        //public Task LogAuthAction(AuthLogType logType, AuthLogData logData);
        void LogInfo(string message); 
        void LogWarn(string message); 
        void LogDebug(string message);
        void LogError(string message);
    }
}
