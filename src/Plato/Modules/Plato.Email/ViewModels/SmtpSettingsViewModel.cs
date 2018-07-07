using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;

namespace Plato.Email.ViewModels
{

    public class EmailSettingsViewModel
    {
        public Pop3SettingsViewModel Pop3Settings { get; set; }

        public SmtpSettingsViewModel SmtpSettings { get; set; }

    }

    public class Pop3SettingsViewModel
    {

        public bool Enabled { get; set; }

        [Required]
        public string ServerName { get; set; }

    }

    public class SmtpSettingsViewModel
    {
        
        public bool Enabled { get; set; } = true;


        [Required]
        [StringLength(255)]
        public bool DefaultFrom { get; set; } = true;
        
        [Required]
        [StringLength(255)]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; } = 25;

        public bool UseTls { get; set; } = true;

        [StringLength(255)]
        public string UserName { get; set; }

        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int SendAttempts { get; set; }

        public int BatchSize { get; set; }

        [Required]
        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        public string PickupDirectoryLocation { get; set; }


    }

}
