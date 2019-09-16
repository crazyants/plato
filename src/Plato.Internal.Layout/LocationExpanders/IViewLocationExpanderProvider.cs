using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Internal.Layout.LocationExpanders
{
    public interface IViewLocationExpanderProvider : IViewLocationExpander
    {
        int Priority { get; }
    }
}
