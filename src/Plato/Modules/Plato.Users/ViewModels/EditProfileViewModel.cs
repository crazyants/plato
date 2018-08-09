using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Plato.Users.ViewModels
{
    public class EditProfileViewModel
    {

        [Required]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string DisplayName { get; set; }

        [DataType(DataType.Text)]
        public string Location { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
        
        [DataType(DataType.Upload)]
        public IFormFile AvatarFile { get; set; }
        
    }
}
