using Microsoft.Extensions.Configuration;

namespace Plato.Internal.Localization.Serializers
{

    public class EmailTemplate
    {

        public string To { get; set; }

        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

    }

    public class EmailSerializer
    {
        public static EmailTemplate Parse(IConfigurationRoot configuration)
        {
            return new EmailTemplate
            {
                To = configuration["To"],
                From = configuration["From"],
                Cc = configuration["Cc"],
                Bcc = configuration["Bcc"],
                Subject = configuration["Subject"],
                Message = configuration["Message"]
            };
        }
    }


}
