using System.Collections.Generic;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class LocaleStrings
    {

        public IEnumerable<LocaleString> KeyValues { get; set; } = new List<LocaleString>();
    }


    public class LocaleString
    {

        public string Key { get; set; }

        public string Value { get; set; }

    }


}
