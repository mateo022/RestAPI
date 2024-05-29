using RestAPIBackendWebService.Business.Logger.Contracts;
using RestAPIBackendWebService.Domain.Auth.Models;
using System.Text.Json;
using RestAPIBackendWebService.Services.Logger.Contract;


namespace RestAPIBackendWebService.Business.Logger.Logic
{
    public class LoggerBusiness : ILoggerBusiness 
    {
        private readonly ILoggerService _loggerService;

        public LoggerBusiness(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public Task LogAuthAction(AuthLogType logType, AuthLogData logData)
        {
            var logDataAsJson = JsonSerializer.Serialize(logData);


            return Task.Run(() => { _loggerService.LogWarn($"{logType.ToString()} {logDataAsJson}"); });
        }

        public void LogDebug(string message) => _loggerService.LogDebug(message);
        public void LogError(string message) => _loggerService.LogError(message);
        public void LogInfo(string message) => _loggerService.LogInfo(message);
        public void LogWarn(string message) => _loggerService.LogWarn(message);
    }
}
