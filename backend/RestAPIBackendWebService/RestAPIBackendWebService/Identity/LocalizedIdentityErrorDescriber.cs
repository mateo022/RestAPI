
using Microsoft.AspNetCore.Identity;
using RestAPIBackendWebService.Domain.Auth.Constants;
using RestAPIBackendWebService.Domain.Services.Localization;
using RestAPIBackendWebService.Domain.User.Constants;

namespace RestAPIBackendWebService.Identity
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = UserRequestLabels.EMAIL,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(DuplicateEmail)], email)
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = UserRequestLabels.USER_NAME,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(DuplicateUserName)], userName)
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = UserRequestLabels.EMAIL,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(InvalidEmail)], email)
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.ROLE,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(DuplicateRoleName)], role)
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.ROLE,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(InvalidRoleName)], role)
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.TOKEN,
                Description = ApplicationTranslations.IdentityErrors[nameof(InvalidToken)]
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = UserRequestLabels.USER_NAME,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(InvalidUserName)], userName)
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = UserRequestLabels.EMAIL,
                Description = ApplicationTranslations.IdentityErrors[nameof(LoginAlreadyAssociated)]
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(PasswordMismatch)]
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(PasswordRequiresDigit)]
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(PasswordRequiresLower)]
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(PasswordRequiresNonAlphanumeric)]
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(PasswordRequiresUniqueChars)], uniqueChars)
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(PasswordRequiresUpper)]
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(PasswordTooShort)], length)
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(UserAlreadyHasPassword)]
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.ROLE,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(UserAlreadyInRole)], role)
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.ROLE,
                Description = string.Format(ApplicationTranslations.IdentityErrors[nameof(UserNotInRole)], role)
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = UserRequestLabels.EMAIL,
                Description = ApplicationTranslations.IdentityErrors[nameof(UserLockoutNotEnabled)]
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = AuthRequestLabels.PASSWORD,
                Description = ApplicationTranslations.IdentityErrors[nameof(RecoveryCodeRedemptionFailed)]
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = UserRequestLabels.COMMON_USER,
                Description = ApplicationTranslations.IdentityErrors[nameof(ConcurrencyFailure)]
            };
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = UserRequestLabels.COMMON_USER,
                Description = ApplicationTranslations.IdentityErrors[nameof(DefaultError)]
            };
        }
    }
}
