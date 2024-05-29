using RestAPIBackendWebService.Domain.Common.DTOs;
using RestAPIBackendWebService.Services.Logger.Contract;
using Newtonsoft.Json;
using System.Net;

namespace RestAPIBackendWebService.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExceptionHandler(RequestDelegate next, ILoggerService logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.UtcNow} - Exception triggered: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorResponse = new ErrorResponseDTO<object>
            {
                Errors = null,
                StatusCode = context.Response.StatusCode,
                Message = $"Falló algo en la apicación {_webHostEnvironment.ApplicationName}. Excepción: {exception.Message}"
            };

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(errorResponse)
            );
        }
    }
}
