using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocDropDownViewComponent : ViewComponent
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityService<Doc> _entityService;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IFeatureFacade _featureFacade;

        public DocDropDownViewComponent(
            IEntityStore<Entity> entityStore,
            IAuthorizationService authorizationService,
            IEntityService<Doc> entityService,
            IFeatureFacade featureFacade)
        {
            _entityStore = entityStore;
            _authorizationService = authorizationService;
            _entityService = entityService;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityDropDownViewModel model)
        {

            if (model == null)
            {
                model = new EntityDropDownViewModel();
            }
            
            model.Entities = await BuildSelectionsAsync(model);
            return View(model);

        }

        private async Task<IList<Selection<Entity>>> BuildSelectionsAsync(EntityDropDownViewModel model)
        {

            if (model.Options.FeatureId == null)
            {
                throw new ArgumentNullException(nameof(model.Options.FeatureId));
            }

            var entities = await GetEntities(model.Options);
            var selections = entities?.Data.Select(e => new Selection<Entity>
                {
                    IsSelected = model.SelectedEntity.Equals(e.Id),
                    Value = e
                })
                .ToList();
            return selections;
        }

        private async Task<IPagedResults<Doc>> GetEntities(EntityIndexOptions options)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");

            // Ensure we found the feature
            if (feature == null)
            {
                throw new Exception("The feature Plato.Docs could not be found");
            }

            if (options.FeatureId <= 0)
            {
                options.FeatureId = feature.Id;
            }

            options.Sort = SortBy.SortOrder;
            options.Order = OrderBy.Asc;

            //var indexOptions = new EntityIndexOptions()
            //{
            //    FeatureId = feature.Id,
            //    Sort = SortBy.SortOrder,
            //    Order = OrderBy.Asc
            //};

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

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenDocs))
                    {
                        q.HideHidden.True();
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
                .GetResultsAsync(options, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

        }

    }

}

