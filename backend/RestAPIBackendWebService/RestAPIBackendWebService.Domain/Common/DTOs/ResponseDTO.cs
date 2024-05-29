namespace RestAPIBackendWebService.Domain.Common.DTOs
{
    public class ResponseDTO : ResponseBase
    {
        public object Data { get; set; }        
    }

    public class ResponseDTO<T> : ResponseBase
    {
        public T Data { get; set; }
    }
}
