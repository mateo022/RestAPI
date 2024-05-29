using RestAPIBackendWebService.Domain.Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestAPIBackendWebService.Domain.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class LocalizationCompareAttribute : CompareAttribute
    {
        public new string OtherPropertyDisplayName { get; private set; }

        public LocalizationCompareAttribute(string otherProperty) : base(otherProperty)
        {
        }

        public override string FormatErrorMessage(string name) =>
            string.Format(
                CultureInfo.CurrentCulture, ApplicationTranslations.DataAnnotationErrors["LocalizationCompareAttribute"], name, OtherPropertyDisplayName ?? OtherProperty);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetRuntimeProperty(OtherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult(string.Format(ApplicationTranslations.DataAnnotationErrors["CompareAttribute_UnknownProperty"], OtherProperty));
            }
            if (otherPropertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException(string.Format(ApplicationTranslations.DataAnnotationErrors["Common_PropertyNotFound"], OtherProperty, validationContext.ObjectType.FullName));
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (!Equals(value, otherPropertyValue))
            {
                if (OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = GetDisplayNameForProperty(otherPropertyInfo);
                }

                string[] memberNames = validationContext.MemberName != null
                   ? new[] { validationContext.MemberName }
                   : null;
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
            }

            return null;
        }

        private string GetDisplayNameForProperty(PropertyInfo property)
        {
            IEnumerable<Attribute> attributes = CustomAttributeExtensions.GetCustomAttributes(property, true);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is DisplayAttribute display)
                {
                    return display.GetName();
                }
            }

            return OtherProperty;
        }
    }
}
