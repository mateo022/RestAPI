using RestAPIBackendWebService.Domain.Auth.DTOs;
using System.ComponentModel.DataAnnotations;


namespace RestAPIBackendWebService.Domain.Auth.DTOs
{
    public class RecoveryPasswordRequestDTO : LoginRequestDTO
    {
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
