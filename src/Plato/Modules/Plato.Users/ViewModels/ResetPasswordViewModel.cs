using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Users.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "new password")]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "new password confirmation")]
        public string PasswordConfirmation { get; set; }

        public bool IsValidResetToken { get; set; }

        public string ResetToken { get; set; }
    }
}
