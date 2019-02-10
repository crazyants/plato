using System.ComponentModel.DataAnnotations;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class StopForumSpamSettingsViewModel
    {

        [Required]
        [StringLength(255)]
        [Display(Name = "site key")]
        public string ApiKey { get; set; }


    }
}
