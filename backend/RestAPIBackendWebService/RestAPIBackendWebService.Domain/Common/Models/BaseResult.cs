using RestAPIBackendWebService.Domain.Common.Errors;

namespace RestAPIBackendWebService.Domain.Common.Models
{
    public class BaseResult
    {
        public bool Success { get; set; } = false;
        public RequestFieldsErrorsCollection<string> ErrorsList { get; set; }
        public BaseResult()
        {
            ErrorsList = new RequestFieldsErrorsCollection<string>();
        }


    }
}
