using System.Collections.Generic;

namespace Plato.Internal.Localization.Abstractions.Models
{
    public class LocaleResources
    {
        public IEnumerable<ComposedLocaleResource> Resources { get; set; } = new List<ComposedLocaleResource>();
    }


}
