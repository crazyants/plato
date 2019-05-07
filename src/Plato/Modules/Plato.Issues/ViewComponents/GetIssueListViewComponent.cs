using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.ViewComponents
{
    public class GetIssueListViewComponent : ViewComponent
    {

        private readonly IEntityService<Issue> _articleService;
        private readonly IAuthorizationService _authorizationService;

        public GetIssueListViewComponent(
            IEntityService<Issue> articleService,
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

        async Task<EntityIndexViewModel<Issue>> GetViewModel(
            EntityIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _articleService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateIssues))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamIssues))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedIssues))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityIndexViewModel<Issue>()
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}



