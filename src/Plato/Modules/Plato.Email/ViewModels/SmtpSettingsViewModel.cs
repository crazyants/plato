using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Plato.Email.ViewModels
{

    public class SmtpSettingsViewModel
    {
 
        [Required]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "default from")]
        public string DefaultFrom { get; set; }
        
        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "host")]
        public string Host { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "port")]
        public int Port { get; set; } = 25;

        public bool EnableSsl { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RequireCredentials { get; set; }

        [Required]
        public SmtpDeliveryMethod DeliveryMethod { get; set; } 

        public string PickupDirectoryLocation { get; set; }
        
        public bool EnablePolling { get; set; } = true;

        [Required]
        [Display(Name = "interval")]
        public int PollInterval { get; set; } = 120;

        [Required]
        [Display(Name = "attempts")]
        public int SendAttempts { get; set; } = 3;

        [Required]
        [Display(Name = "batch size")]
        public int BatchSize { get; set; } = 50;

    }

}
