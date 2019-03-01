using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class GetTopicListViewComponent : ViewComponent
    {
        
        private readonly IEntityService<Topic> _entityService;

        public GetTopicListViewComponent(
            IEntityService<Topic> entityService, 
            IFeatureFacade featureFacade)
        {
            _entityService = entityService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Build default
            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            // Build default
            if (pager == null)
            {
                pager = new PagerOptions();
            }
        
            // Review view
            return View(await GetViewModel(options, pager));

        }
        
        async Task<EntityIndexViewModel<Topic>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _entityService.GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new EntityIndexViewModel<Topic>
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

