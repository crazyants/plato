using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.LocaleSerializers
{

    public class LocaleEmailsSerializer
    {

        private const string SectionName = "Emails";

        public static IEnumerable<LocaleEmail> Parse(IConfigurationRoot configuration)
        {

            var templates = new List<LocaleEmail>();
            var section = configuration.GetSection(SectionName);
            if (section != null)
            {
                var children = section.GetChildren();
                foreach (var child in children)
                {
                    templates.Add(new LocaleEmail
                    {
                        Key = child["Id"],
                        To = child["To"],
                        From = child["From"],
                        Cc = child["Cc"],
                        Bcc = child["Bcc"],
                        Subject = child["Subject"],
                        Message = child["Message"]
                    });
                }

            }

            return templates;

        }

    }

}
