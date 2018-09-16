using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Search.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {

        public SearchViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View());
        }

    }

}
