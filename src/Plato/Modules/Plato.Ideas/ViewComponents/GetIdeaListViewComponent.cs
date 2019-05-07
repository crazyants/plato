using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.ViewComponents
{
    public class GetIdeaListViewComponent : ViewComponent
    {
        
        private readonly IEntityService<Idea> _articleService;
        private readonly IAuthorizationService _authorizationService;

        public GetIdeaListViewComponent(
            IEntityService<Idea> articleService,
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
        
        async Task<EntityIndexViewModel<Idea>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _articleService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateIdeas))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamIdeas))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedIdeas))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new EntityIndexViewModel<Idea>()
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}







