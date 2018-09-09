using System.Threading.Tasks;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization.Abstractions
{

    public interface ILocaleCompositionStrategy
    {

        Task<ComposedLocaleDescriptor> ComposeLocaleDescriptorAsync(LocaleDescriptor descriptor);

    }

}
