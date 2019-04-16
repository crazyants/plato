using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Microsoft.AspNetCore.Authorization;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.ViewComponents
{
   public class DocTreeViewComponent : ViewComponent
    {

        private readonly IEntityService<Doc> _entityService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFeatureFacade _featureFacade;

        public DocTreeViewComponent(
            IEntityService<Doc> entityService,
            IAuthorizationService authorizationService,
            IFeatureFacade featureFacade)
        {
            _entityService = entityService;
            _authorizationService = authorizationService;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityTreeOptions options)
        {

            // Get entities
            var entities = await GetEntities();

            // Add entities to view model
            options.Entities = entities?.Data?.BuildHierarchy<Doc>()?.OrderBy(r => r.SortOrder);
            options.EditMenuViewName = await DisplayMenu() ? "DocTreeMenu" : string.Empty;

            // Return view
            return View(options);

        }

        private async Task<IPagedResults<Doc>> GetEntities()
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
            if (feature == null)
            {
                throw new Exception("The feature Plato.Docs could not be found");
            }

            var indexOptions = new EntityIndexOptions()
            {
                FeatureId = feature.Id,
                Sort = SortBy.SortOrder,
                Order = OrderBy.Asc
            };

            // Get results
            return await _entityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateDocs))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamDocs))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedDocs))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(indexOptions, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

        }

        private async Task<bool> DisplayMenu()
        {

            // Permissions needed for the menu
            var permissions = new List<IPermission>
            {
                Permissions.PostDocs,
                Permissions.EditAnyDoc,
                Permissions.SortAnyDoc,
                Permissions.DeleteAnyDoc
            };

            // If any of the permissions are allowed display menu
            foreach (var permission in permissions)
            {
                if (await _authorizationService.AuthorizeAsync(ViewContext.HttpContext.User, permission))
                {
                    return true;
                }
            }

            return false;

        }

    }

}
