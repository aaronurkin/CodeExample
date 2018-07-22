using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace AaronUrkinCodeExample.BusinessLogicLayer.Services.Security
{
    /// <summary>
    /// Translates <see cref="IdentityError"/>s desctiptions
    /// </summary>
    public class ApplicationIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<ApplicationIdentityErrorDescriber> translate;

        public ApplicationIdentityErrorDescriber(IStringLocalizer<ApplicationIdentityErrorDescriber> localizer)
        {
            this.translate = localizer;
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = this.translate["Concurrency failure"]
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = this.translate["Email {0} is already taken.", email]
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = this.translate["UserName is already taken."]
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = this.translate["Role {0} already exists.", role]
            };
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = this.translate["An error has occured."]
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = this.translate["Email {0} is invalid. Please, enter correct Email", email]
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = this.translate["Role name {0} is invalid.", role]
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = this.translate["Token is invalid"]
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = this.translate["UserName {0} is invalid", userName]
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = this.translate["The password and confirmation password do not match."]
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = nameof(LoginAlreadyAssociated),
                Description = this.translate["Login is already associated"]
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = this.translate["Passwords must have at least one digit ('0'-'9')."]
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = this.translate["Passwords must have at least one lowercase ('a'-'z')."]
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = this.translate["Passwords must have at least one non alphanumeric character."]
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = this.translate["Passwords must have unique characters."]
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = this.translate["Passwords must have at least one uppercase ('A'-'Z')."]
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = this.translate["Password must be more than {0} characters long", length - 1]
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = this.translate["Recovery code redemption failed."]
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = this.translate["The user already has password"]
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = this.translate["The user is already in role {0}", role]
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = this.translate["User lockout not enabled"]
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = this.translate["The user is not in role {0}", role]
            };
        }
    }
}
