using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Internal.Layout.LocationExpander
{
    public interface IViewLocationExpanderProvider : IViewLocationExpander
    {
        int Priority { get; }
    }
}
