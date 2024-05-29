using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestAPIBackendWebService.Domain.Auth.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Revisa bien tú correo")]
        public string Email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(8 ,ErrorMessage = "La contraseña debe tener minímo 8 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
