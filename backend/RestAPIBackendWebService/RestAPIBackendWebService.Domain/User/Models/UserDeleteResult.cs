
using RestAPIBackendWebService.Domain.Common.Models;

namespace RestAPIBackendWebService.Domain.User.Models
{
    public class UserDeleteResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }

        public UserDeleteResult()
        {
            this.Errors = new List<string>();
        }
    }
}
