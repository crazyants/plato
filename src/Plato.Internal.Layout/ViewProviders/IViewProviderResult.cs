using System.Collections.Generic;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout.ViewProviders
{

    public interface IViewProviderResult
    {
        IEnumerable<IView> Views { get; }
    }

}
