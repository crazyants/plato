using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Features.ViewModels;
using Plato.Site.ViewModels;

namespace Plato.Site.ViewComponents
{

    public class SiteHeaderViewComponent : ViewComponent
    {
        
        public SiteHeaderViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            bool sticky)
        {

            return Task.FromResult((IViewComponentResult) View(new SiteHeaderViewModel()
            {
                Sticky = sticky
            }));
        }

    }

}
