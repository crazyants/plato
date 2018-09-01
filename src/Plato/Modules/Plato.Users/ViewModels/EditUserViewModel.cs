using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Plato.Users.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        public string Location { get; set; }

        public string Bio { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

        public bool DisplayPasswordFields { get; set; }
        
        [DataType(DataType.Upload)]
        public IFormFile AvatarFile { get; set; }

        public IEnumerable<string> RoleNames { get; set; }

        public bool IsNewUser { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }


    }
}
