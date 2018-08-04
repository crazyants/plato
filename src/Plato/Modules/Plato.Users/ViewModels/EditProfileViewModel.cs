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
        public string DisplayName { get; set; }

        public string Location { get; set; }


        public string Bio { get; set; }


        [DataType(DataType.Upload)]
        public IFormFile AvatarFile { get; set; }


    }
}
