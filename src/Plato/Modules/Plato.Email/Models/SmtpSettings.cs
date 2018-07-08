using System.Net.Mail;

namespace Plato.Email.Models
{
 
    public class SmtpSettings
    {

        public bool EnablePolling { get; set; } = true;

        public string DefaultFrom { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 25;

        public bool EnableSsl { get; set; } = true;
        
        public string UserName { get; set; }

        public string Password { get; set; }

        public int SendAttempts { get; set; }

        public int PollingInterval { get; set; }
        
        public int BatchSize { get; set; }

        public SmtpDeliveryMethod DeliveryMethod { get; set; } = SmtpDeliveryMethod.Network;

        public string PickupDirectoryLocation { get; set; }

        public bool RequireCredentials { get; set; }

        public bool UseDefaultCredentials { get; set; }

        
    }

}
