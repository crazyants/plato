using Microsoft.AspNetCore.Mvc.Filters;

namespace Plato.Internal.Layout.ActionFilters
{
    public interface IModularActionFilter : IActionFilter, IAsyncResultFilter
    {
    }
}
