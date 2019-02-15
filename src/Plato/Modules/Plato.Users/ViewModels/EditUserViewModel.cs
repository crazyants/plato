using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Models.Users;

namespace Plato.Users.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "display name")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "email address")]
        public string Email { get; set; }
        
        public string Location { get; set; }

        public string Bio { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile AvatarFile { get; set; }

        public IEnumerable<string> RoleNames { get; set; }
        
        public DateTimeOffset? CreatedDate { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }

        // -------

        public UserAvatar Avatar { get; set; }
        
        public bool IsNewUser { get; set; }
        
        public bool DisplayPasswordFields { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool IsSpam { get; set; }

        public SimpleUser IsSpamUpdatedUser { get; set; }

        public DateTimeOffset? IsSpamUpdatedDate { get; set; }
        
        public bool IsVerified { get; set; }

        public SimpleUser IsVerifiedUpdatedUser { get; set; }

        public DateTimeOffset? IsVerifiedUpdatedDate { get; set; }


        public bool IsBanned { get; set; }

        public SimpleUser IsBannedUpdatedUser { get; set; }

        public DateTimeOffset? IsBannedUpdatedDate { get; set; }


    }

}
