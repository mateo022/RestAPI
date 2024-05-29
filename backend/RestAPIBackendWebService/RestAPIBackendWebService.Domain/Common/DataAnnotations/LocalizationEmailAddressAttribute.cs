using RestAPIBackendWebService.Domain.Services.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RestAPIBackendWebService.Domain.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class LocalizationEmailAddressAttribute : DataTypeAttribute
    {
        private const string EMAIL_VALIDATION_PATTERN = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public LocalizationEmailAddressAttribute() : base(DataType.EmailAddress)
        {
            ErrorMessage = ApplicationTranslations.DataAnnotationErrors[this.GetType().Name] ?? "";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is string valueAsString))
            {
                return false;
            }

            if (valueAsString.Contains('\r') || valueAsString.Contains('\n'))
            {
                return false;
            }

            return Regex.Match(valueAsString, EMAIL_VALIDATION_PATTERN, RegexOptions.IgnoreCase).Success;
        }
    }
}
