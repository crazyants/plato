using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.ViewComponents
{
    public class GetArticleListViewComponent : ViewComponent
    {
        
        private readonly IEntityService<Article> _articleService;
        private readonly IAuthorizationService _authorizationService;

        public GetArticleListViewComponent(
            IEntityService<Article> articleService,
            IAuthorizationService authorizationService)
        {
            _articleService = articleService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityIndexOptions options, PagerOptions pager)
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
        
        async Task<EntityIndexViewModel<Article>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _articleService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateArticles))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamArticles))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedArticles))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);

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

