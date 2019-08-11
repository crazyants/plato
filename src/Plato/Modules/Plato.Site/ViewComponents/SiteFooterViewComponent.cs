using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Features.ViewModels;

namespace Plato.Site.ViewComponents
{

    public class SiteFooterViewComponent : ViewComponent
    {
        
        public SiteFooterViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(FeatureIndexOptions options)
        {

            return Task.FromResult((IViewComponentResult) View());
        }

    }

}
