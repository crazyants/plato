using System.Collections.Generic;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class ComposedLocaleDescriptor
    {

        public LocaleDescriptor Descriptor { get; set; }

        public IEnumerable<ComposedLocaleResource> Resources { get; set; }

    }

}
