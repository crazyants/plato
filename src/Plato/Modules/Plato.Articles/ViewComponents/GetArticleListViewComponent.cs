using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.ViewComponents
{
    public class GetArticleListViewComponent : ViewComponent
    {

        private readonly IFeatureFacade _featureFacade;
        private readonly IEntityService<Article> _articleService;

        public GetArticleListViewComponent(
            IEntityService<Article> articleService,
            IFeatureFacade featureFacade)
        {
            _articleService = articleService;
            _featureFacade = featureFacade;
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

            // Explicitly set feature Id
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }
            
            // Review view
            return View(await GetViewModel(options, pager));

        }
        
        async Task<EntityIndexViewModel<Article>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _articleService.GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new EntityIndexViewModel<Article>()
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

