using Microsoft.Extensions.Localization;

namespace Plato.Internal.Localization.Abstractions.Models
{
    
    public class LocaleString : LocalizedString, ILocalizedValue
    {

        public new string Name { get; set; }
        
        public LocaleString(string name, string value) : base(name, value)
        {
            this.Name = name;
        }

        public LocaleString(string name, string value, bool resourceNotFound) : base(name, value, resourceNotFound)
        {
            this.Name = name;
        }

        public LocaleString(string name, string value, bool resourceNotFound, string searchedLocation) : base(name, value, resourceNotFound, searchedLocation)
        {
            this.Name = name;
        }

    }

}
