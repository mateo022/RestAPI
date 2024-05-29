using Newtonsoft.Json;


namespace RestAPIBackendWebService.Domain.Auth.DTOs
{
    public class TwoAuthRequestDTO
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}
