using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Docs.Models;
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
            var results = await _entityService
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
       
            return View(new EntityTreeOptions()
            {
                Entities = results?.Data,
                EditMenuViewName = "",
                SelectedEntity = options.SelectedEntity,
                EnableCheckBoxes = options.EnableCheckBoxes,
                CssClass = options.CssClass,
                RouteValues = new RouteValueDictionary()
                {
                    ["area"] = "Plato.Docs",
                    ["controller"] = "Home",
                    ["action"] = "Display"
                }
            });

        }
        
    }

}
