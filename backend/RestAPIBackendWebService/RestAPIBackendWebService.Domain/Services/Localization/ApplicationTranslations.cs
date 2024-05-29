using System.Collections.Immutable;

namespace RestAPIBackendWebService.Domain.Services.Localization
{
    public static class ApplicationTranslations
    {
        private static Dictionary<string, string> _identityErrors;
        private static Dictionary<string, string> _dataAnnotationsErrors;

        public static ImmutableDictionary<string, string> IdentityErrors
        {
            get { 
                if(_identityErrors != null) 
                    return _identityErrors.ToImmutableDictionary<string, string>();

                throw new Exception("Error when loading translations: IdentityErrors translations are empty");
            }
        }

        public static ImmutableDictionary<string, string> DataAnnotationErrors
        {
            get
            {
                if (_dataAnnotationsErrors != null)
                    return _dataAnnotationsErrors.ToImmutableDictionary<string, string>();

                throw new Exception("Error when loading translations: DataAnnotationErrors translations are empty");
            }
        }

        public static void InitializeTranslations(
            Dictionary<string, string> identityErrors,
            Dictionary<string, string> dataAnnotations)
        {
            _identityErrors = identityErrors;
            _dataAnnotationsErrors = dataAnnotations;
        }
    }
}
