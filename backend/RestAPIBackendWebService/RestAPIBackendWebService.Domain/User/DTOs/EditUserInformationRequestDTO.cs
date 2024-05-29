using RestAPIBackendWebService.Domain.Common.DataAnnotations;



namespace RestAPIBackendWebService.Domain.User.DTOs
{
    public class EditUserInformationRequestDTO : UserEditBase
    {
        [LocalizationRequired]
        public string Name { get; set; }
        public new string Password { get; set; }
        public new string Email { get; set; }

        [LocalizationCompare(nameof(Password))]
        public new string ConfirmPassword { get; set; }
    }
}
