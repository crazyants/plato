using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchViewComponent : ViewComponent
    {

        public SearchViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync(
            ViewOptions viewOpts)
        {

            if (viewOpts == null)
            {
                viewOpts = new ViewOptions();
            }
            
            return Task.FromResult((IViewComponentResult)View(viewOpts));
        }

    }

}
