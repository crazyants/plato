using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Plato.Site.ViewModels
{
    public class ContactFormViewModel
    {

        [Required, Display(Name = "name")]
        public string Name { get; set; }

        [Required, DataType(DataType.EmailAddress), Display(Name = "email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Text), Display(Name = "subject")]
        public string Subject { get; set; }

        [Required, DataType(DataType.MultilineText), Display(Name = "message")]
        public string Message { get; set; }

    }

}
