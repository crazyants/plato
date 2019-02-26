using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListItemViewComponent : ViewComponent
    {

        public SearchListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(SearchListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));

        }

    }
    
}

