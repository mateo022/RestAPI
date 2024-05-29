
using RestAPIBackendWebService.Domain.Services.Localization;
using System.ComponentModel.DataAnnotations;

namespace RestAPIBackendWebService.Domain.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class LocalizationRequiredAttribute : ValidationAttribute
    {
        public bool AllowEmptyStrings { get; set; }

        public LocalizationRequiredAttribute() : base()
        {
            ErrorMessage = ApplicationTranslations.DataAnnotationErrors[this.GetType().Name];
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            // only check string length if empty strings are not allowed
            return AllowEmptyStrings || !(value is string stringValue) || !string.IsNullOrWhiteSpace(stringValue);
        }
    }
}
