using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.LocaleSerializers
{

    public class LocaleEmailsSerializer
    {

        public static LocaleEmails Parse(IConfigurationRoot configuration)
        {

            var templates = new List<LocaleEmail>();
            var emails = configuration.GetSection("Emails");
            var children = emails.GetChildren();
            foreach (var child in children)
            {
                templates.Add(new LocaleEmail
                {
                    To = child.GetValue<string>("To"),
                    From = child["From"],
                    Cc = child["Cc"],
                    Bcc = child["Bcc"],
                    Subject = child["Subject"],
                    Message = child["Message"]
                });
            }

            return new LocaleEmails()
            {
                Templates = templates
            };

        }

    }

}
