using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Security.Attributes
{

    /// <summary>
    /// A custom view model validation attribute to ensure user passwords match configured ASP.NET identity options.
    /// </summary>
    public class IdentityPasswordOptionsValidator : ValidationAttribute
    {
                
        public IdentityErrorDescriber Describer { get; set; } = new IdentityErrorDescriber();

        public IdentityPasswordOptionsValidator()
        {    
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {

            // Get password
            var password = ((string)value);

            var identityOptions = (IOptions<IdentityOptions>) context.GetService(typeof(IOptions<IdentityOptions>));
            var result = ValidatePassword(password, identityOptions.Value);

            // Return the first validation error we encounter
            if (!result.Succeeded)
            {             
                foreach (var error in result.Errors)
                {
                    return new ValidationResult(error.Description);
                }              
            }
                       
            return ValidationResult.Success;
        }
        
        // -------------


        IdentityResult ValidatePassword(string password, IdentityOptions identityOptions)
        {

            var errors = new List<IdentityError>();
            var options = identityOptions.Password;

            if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
            {
                errors.Add(Describer.PasswordTooShort(options.RequiredLength));
            }

            if (options.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
            {
                errors.Add(Describer.PasswordRequiresNonAlphanumeric());
            }

            if (options.RequireDigit && !password.Any(IsDigit))
            {
                errors.Add(Describer.PasswordRequiresDigit());
            }

            if (options.RequireLowercase && !password.Any(IsLower))
            {
                errors.Add(Describer.PasswordRequiresLower());
            }

            if (options.RequireUppercase && !password.Any(IsUpper))
            {
                errors.Add(Describer.PasswordRequiresUpper());
            }

            if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
            {
                errors.Add(Describer.PasswordRequiresUniqueChars(options.RequiredUniqueChars));
            }

            return
                errors.Count == 0
                    ? IdentityResult.Success
                    : IdentityResult.Failed(errors.ToArray());

        }

        public virtual bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public virtual bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        public virtual bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        public virtual bool IsLetterOrDigit(char c)
        {
            return IsUpper(c) || IsLower(c) || IsDigit(c);
        }

    }

}
