using System.Collections.Generic;

namespace Plato.Internal.Localization.Abstractions.Models
{

    public class LocalizedValues<TModel> where TModel : class, ILocalizedValue
    {
        public LocaleResource Resource { get; set; }

        public IEnumerable<TModel> Values { get; set; } = new List<TModel>();

    }

}
