using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Users.ViewModels
{
    public class ActivateAccountViewModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

        public bool IsValidConfirmationToken { get; set; }

        public string ConfirmationToken { get; set; }
    }
}
