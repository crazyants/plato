using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Services;
using Plato.Articles.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.ViewComponents
{
    public class GetArticleListViewComponent : ViewComponent
    {
        
        private readonly IArticleService _articleService;

        public GetArticleListViewComponent(
            IArticleService articleService)
        {
            _articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ArticleIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new ArticleIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }
        
        async Task<ArticleIndexViewModel> GetViewModel(
            ArticleIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _articleService.GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new ArticleIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

