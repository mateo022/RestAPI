

using RestAPIBackendWebService.Domain.Services.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RestAPIBackendWebService.Domain.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class LocalizationPhoneIndicatorAttribute : ValidationAttribute
    {
        private const string PHONE_INDICATOR_VALIDATION_PATTERN = "^(\\+\\d{1,4})$";

        public LocalizationPhoneIndicatorAttribute() : base()
        {
            ErrorMessage = ApplicationTranslations.DataAnnotationErrors[this.GetType().Name];
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            return Regex.Match(value as string, PHONE_INDICATOR_VALIDATION_PATTERN, RegexOptions.IgnoreCase).Success;
        }
    }
}
