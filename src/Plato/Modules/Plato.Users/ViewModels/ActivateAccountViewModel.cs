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
        public string Email { get; set; }

        public bool IsValidConfirmationToken { get; set; }

        public string ConfirmationToken { get; set; }
    }
}
