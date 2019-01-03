using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.Services;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Labels.ViewComponents
{

    public class GetLabelListViewComponent : ViewComponent
    {
        
        private readonly ILabelService _labelService;

        public GetLabelListViewComponent(
            ILabelService labelService)
        {
            _labelService = labelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            LabelIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            return View(await GetViewModel(options, pager));

        }

        async Task<LabelIndexViewModel> GetViewModel(
            LabelIndexOptions options,
            PagerOptions pager)
        {

            var results = await _labelService.GetLabelsAsunc(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new LabelIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }


}
