using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Abstractions
{
    public interface ILocaleProvider
    {
        Task<IEnumerable<ComposedLocaleDescriptor>> GetLocalesAsync();

    }

}
