using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Features.Abstractions;

namespace Plato.Features.Updates.ViewComponents
{

    public class FeatureUpdateListViewComponent : ViewComponent
    {
        
        private readonly IFeatureFacade _featureFacade; 

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public FeatureUpdateListViewComponent(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IFeatureFacade featureFacade)
        {
            T = htmlLocalizer;
            S = stringLocalizer;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(FeatureUpdateOptions options)
        {
            return View(new FeatureUpdatesViewModel()
            {
                Options = options,
                Features = await _featureFacade.GetFeatureUpdatesAsync()
            });

        }
        
    }

}
