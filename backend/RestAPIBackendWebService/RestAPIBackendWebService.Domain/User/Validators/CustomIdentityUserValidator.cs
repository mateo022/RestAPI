
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestAPIBackendWebService.Domain.User.Validators
{
    public class CustomIdentityUserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
        public IdentityErrorDescriber Describer { get; private set; }

        public CustomIdentityUserValidator(IdentityErrorDescriber errors = null)
        {
           Describer  = errors ?? new IdentityErrorDescriber();
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var errors = await ValidateUserName(manager, user).ConfigureAwait(false);
            if (manager.Options.User.RequireUniqueEmail)
            {
                errors = await ValidateEmail(manager, user, errors).ConfigureAwait(false);
            }

            return errors?.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task<List<IdentityError>> ValidateUserName(UserManager<TUser> manager, TUser user)
        {
            List<IdentityError> errors = null;
            var userName = await manager.GetUserNameAsync(user).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(userName))
            {
                errors ??= new List<IdentityError>();
                errors.Add(Describer.InvalidUserName(userName));
            }
            else if (!string.IsNullOrEmpty(manager.Options.User.AllowedUserNameCharacters) &&
                userName.Any(c => !manager.Options.User.AllowedUserNameCharacters.Contains(c)))
            {
                errors ??= new List<IdentityError>();
                errors.Add(Describer.InvalidUserName(userName));
            }

            return errors;
        }

        // make sure email is not empty, valid, and unique
        private async Task<List<IdentityError>> ValidateEmail(UserManager<TUser> manager, TUser user, List<IdentityError> errors)
        {
            var email = await manager.GetEmailAsync(user).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors ??= new List<IdentityError>();
                errors.Add(Describer.InvalidEmail(email));
                return errors;
            }
            if (!new EmailAddressAttribute().IsValid(email))
            {
                errors ??= new List<IdentityError>();
                errors.Add(Describer.InvalidEmail(email));
                return errors;
            }
            var owner = await manager.FindByEmailAsync(email).ConfigureAwait(false);
            if (owner != null &&
                !string.Equals(await manager.GetUserIdAsync(owner).ConfigureAwait(false), await manager.GetUserIdAsync(user).ConfigureAwait(false)))
            {
                errors ??= new List<IdentityError>();
                errors.Add(Describer.DuplicateEmail(email));
            }
            return errors;
        }
    }
}
