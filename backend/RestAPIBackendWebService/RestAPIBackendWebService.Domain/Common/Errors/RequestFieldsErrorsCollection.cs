using Microsoft.AspNetCore.Identity;
namespace RestAPIBackendWebService.Domain.Common.Errors
{
    public class RequestFieldsErrorsCollection<TError>
    {
        private Dictionary<string, List<TError>> _errors = new Dictionary<string, List<TError>>();

        public List<TError> this[string index]
        {
            get { return _errors[index]; }
        }

        public Dictionary<string, List<TError>> Collection
        {
            get { return _errors; }
        }

        public void AddErrorForKey(string fieldKey, TError error)
        {
            var hasKey = _errors.TryGetValue(fieldKey, out List<TError> errorsList);

            if(errorsList != null && hasKey)
            {
                errorsList.Add(error);
            }
            else
            {
                errorsList = new List<TError> { error };
                _errors.Add(fieldKey, errorsList);
            }
        }

        public void AddErrorsForKey(string fieldKey, List<TError> errors)
        {
            var hasKey = _errors.TryGetValue(fieldKey, out List<TError> errorsList);

            if (errorsList != null && hasKey)
            {
                errorsList.AddRange(errors);
            }
            else
            {
                _errors.Add(fieldKey, errors);
            }
        }
        public void AddErrorsMessagesFromIdentityResult(IdentityResult identityResult,
            Func<IdentityError, TError> selectableOfIdentityErros,
            string errKey)
        {
            if (!identityResult.Succeeded)
            {
                AddErrorsForKey(errKey, identityResult.Errors.Select(selectableOfIdentityErros).ToList());
            }
        }
    }
    public class RequestFieldsErrorsCollection
    {
        public static Dictionary<string, List<string>> MapIdentityErrorsByCommonKey(IEnumerable<IdentityError> identityErrors)
        {
            return identityErrors.GroupBy(g => g.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());
        }
    }
}
