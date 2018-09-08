using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleManager
    {

        Task<IEnumerable<ComposedLocaleDescriptor>> Locales(IEnumerable<string> paths = null);

    }

}
