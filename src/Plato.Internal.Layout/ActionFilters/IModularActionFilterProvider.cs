using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Plato.Internal.Layout.ActionFilters
{

    public interface IModularActionFilter : IActionFilter
    {
        Task OnActionExecutingAsync(ResultExecutingContext context);

        Task OnActionExecutedAsync(ResultExecutingContext context);

    }
}
