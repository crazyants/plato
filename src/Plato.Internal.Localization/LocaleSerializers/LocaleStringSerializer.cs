using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.LocaleSerializers
{

    public class LocaleStringSerializer
    {
        public static IEnumerable<LocaleString> Parse(IConfigurationRoot configuration)
        {
     
            var values = new List<LocaleString>();
            var section = configuration.GetSection("Strings");
            if (section != null)
            {
                foreach (var child in section.GetChildren())
                {
                    values.Add(new LocaleString(child["Key"], child["Value"]));
                }
            }

            return values;

        }

    }

}
