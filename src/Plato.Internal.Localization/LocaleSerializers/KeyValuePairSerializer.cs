using Microsoft.Extensions.Configuration;

namespace Plato.Internal.Localization.LocaleSerializers
{

    public class KeyValuePair
    {

        public string Key { get; set; }

        public string Value { get; set; }

    }

    public class KeyValuePairSerializer
    {
        public static KeyValuePair Parse(IConfigurationRoot configuration)
        {
            var output = new KeyValuePair
            {
                Key = configuration["Key"],
                Value = configuration["Value"]
            };
            return output;
        }
    }
}
