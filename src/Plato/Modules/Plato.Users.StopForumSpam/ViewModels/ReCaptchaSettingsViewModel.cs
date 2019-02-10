using System.ComponentModel.DataAnnotations;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class StopForumSpamSettingsViewModel
    {

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "site key")]
        public string ApiKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "username threshold")]
        public int UserNameThreshold { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "email threshold")]
        public int EmailThreshold { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ip address threshold")]
        public int IpAddressThreshold { get; set; }
        
    }

}

