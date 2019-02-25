using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchMenuViewComponent : ViewComponent
    {

        public SearchMenuViewComponent()
        {

        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityIndexOptions options)
        {

            if (options == null)
            {
                options = new EntityIndexOptions();
            }
            
            return Task.FromResult((IViewComponentResult)View(options));
        }

    }

}
