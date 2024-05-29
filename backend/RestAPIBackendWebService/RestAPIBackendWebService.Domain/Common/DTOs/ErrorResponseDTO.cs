namespace RestAPIBackendWebService.Domain.Common.DTOs
{
    public class ErrorResponseDTO<TErrorCollection> : ResponseBase
    {
        public TErrorCollection Errors { get; set; }
    }

    public class ErrorResponseDTO : ResponseBase { }
}
