namespace RestAPIBackendWebService.Domain.Auth.DTOs
{
    public class VerifyTwoFactorCodeRequestDTO
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }
}
