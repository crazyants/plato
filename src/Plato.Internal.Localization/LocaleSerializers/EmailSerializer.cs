using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Plato.Internal.Localization.LocaleSerializers
{

    public class EmailTemplates
    {
        public IEnumerable<EmailTemplate> Templates { get; set; } = new List<EmailTemplate>();

    }

    public class EmailTemplate
    {

        public string Id { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

    }

    public class EmailSerializer
    {
        public static EmailTemplates Parse(IConfigurationRoot configuration)
        {
            
            var templates = new List<EmailTemplate>();
            var emails = configuration.GetSection("Emails");
            var children = emails.GetChildren();
            foreach (var child in children)
            {
                templates.Add(new EmailTemplate
                {
                    To = child.GetValue<string>("To"),
                    From = child["From"],
                    Cc = child["Cc"],
                    Bcc = child["Bcc"],
                    Subject = child["Subject"],
                    Message = child["Message"]
                });
            }

            return new EmailTemplates()
            {
                Templates = templates
            };

        }

    }

}
