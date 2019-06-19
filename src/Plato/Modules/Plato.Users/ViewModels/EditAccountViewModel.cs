using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Users.ViewModels
{
    public class EditAccountViewModel
    {

        public int Id { get; set; }
        
        [Required, StringLength(255, MinimumLength = 4)]
        [RegularExpression("[^\\n|^\\s|^,|^@]*", ErrorMessage = "The username cannot contain the @ or , characters.")]
        public string UserName { get; set; }
        
        [Required, EmailAddress, DataType(DataType.EmailAddress), Display(Name = "email")]
        [StringLength(255, MinimumLength = 4)]
        public string Email { get; set; }

    }
}
