
using RestAPIBackendWebService.Domain.Services.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RestAPIBackendWebService.Domain.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class LocalizationPhoneAttribute : DataTypeAttribute
    {
        private const string PHONE_VALIDATION_PATTERN = "^(\\d{7,15})$";

        public LocalizationPhoneAttribute() : base(DataType.PhoneNumber)
        {
            ErrorMessage = ApplicationTranslations.DataAnnotationErrors[this.GetType().Name];
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            return Regex.Match(value as string, PHONE_VALIDATION_PATTERN, RegexOptions.IgnoreCase).Success;
        }
    }
}
