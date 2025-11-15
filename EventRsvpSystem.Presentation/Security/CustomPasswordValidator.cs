using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace EventRsvpSystem.Presentation.Security;

public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
{
    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordEmpty",
                Description = "Password cannot be empty."
            }));
        }

        int uppercaseCount = password.Count(char.IsUpper);
        int digitCount = password.Count(char.IsDigit);
        int symbolCount = password.Count(c => !char.IsLetterOrDigit(c));

        var errors = new List<IdentityError>();

        if (uppercaseCount < 2)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordUppercase",
                Description = "Password must contain at least 2 uppercase letters."
            });
        }

        if (digitCount < 3)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordDigits",
                Description = "Password must contain at least 3 digits."
            });
        }

        if (symbolCount < 3)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordSymbols",
                Description = "Password must contain at least 3 symbols (non-letter, non-digit)."
            });
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
