using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Features.ViewModels;

namespace Plato.Site.ViewComponents
{

    public class SiteFooterMinimalViewComponent : ViewComponent
    {
        
        public SiteFooterMinimalViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(FeatureIndexOptions options)
        {

            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
