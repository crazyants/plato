using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Models.Users;

namespace Plato.Users.ViewModels
{
    public class EditProfileViewModel
    {

        [Required]
        public int Id { get; set; }

        [Required, DataType(DataType.Text), StringLength(255)]
        public string DisplayName { get; set; }

        [DataType(DataType.Text), StringLength(255)]
        public string Location { get; set; }

        [DataType(DataType.Url), StringLength(255)]
        public string Url { get; set; }

        public UserAvatar Avatar { get; set; }

        [DataType(DataType.MultilineText), StringLength(255)]
        public string Biography { get; set; }
        
        [DataType(DataType.Upload)]
        public IFormFile AvatarFile { get; set; }
        
    }
}
