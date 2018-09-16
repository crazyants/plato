using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Core.ViewComponents
{
    public class BaseSearchViewComponent : ViewComponent
    {

        public BaseSearchViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View());
        }

    }

}
