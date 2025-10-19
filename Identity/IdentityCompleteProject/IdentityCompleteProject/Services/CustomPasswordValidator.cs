using Microsoft.AspNetCore.Identity;
using IdentityCompleteProject.Models;

namespace IdentityCompleteProject.Services
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            var errors = new List<IdentityError>();

            // Check if password contains username
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsUserName",
                    Description = "Password cannot contain your username"
                });
            }

            // Check if password contains email
            if (password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsEmail",
                    Description = "Password cannot contain your email address"
                });
            }

            // Check for sequential numbers
            if (ContainsSequentialNumbers(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordSequentialNumbers",
                    Description = "Password cannot contain sequential numbers"
                });
            }

            // Check for repeated characters
            if (ContainsRepeatedCharacters(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRepeatedChars",
                    Description = "Password cannot contain repeated characters"
                });
            }

            return errors.Count == 0
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        private bool ContainsSequentialNumbers(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (char.IsDigit(password[i]) &&
                    char.IsDigit(password[i + 1]) &&
                    char.IsDigit(password[i + 2]))
                {
                    if (password[i + 1] == password[i] + 1 &&
                        password[i + 2] == password[i] + 2)
                        return true;
                }
            }
            return false;
        }

        private bool ContainsRepeatedCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] &&
                    password[i] == password[i + 2])
                    return true;
            }
            return false;
        }
    }
}