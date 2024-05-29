using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Contracts;
using RestAPIBackendWebService.Domain.Auth.DTOs;
using System.Net.Http.Headers;
using System.Text;


namespace RestAPIBackendWebService.DataAccess.APIs.Softipal.Auth.Logic
{
    public class TwoAuth : ITwoAuth
    {
        private readonly IConfiguration _configuration;

        public TwoAuth(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<bool> TwoFactorCode(TwoAuthRequestDTO requestBody)
        {
            var _URLCode = _configuration["URLTwoFactorCode"];

            string contentType = "application/json";
            bool responseObj = false;


            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, _URLCode))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, contentType);
                    request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(contentType);

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.SendAsync(request).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = !response.Content.ReadAsStringAsync().Result.Equals("Invalid Token");
                    }
                    response.Dispose();
                }
            }

            return responseObj; 
        }
    }
}
