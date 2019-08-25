using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocViewComponent : ViewComponent
    {

        private readonly IEntityService<Doc> _entityService;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IAuthorizationService _authorizationService;

        public DocViewComponent(
            IEntityStore<Doc> entityStore,
            IEntityService<Doc> entityService,
            IAuthorizationService authorizationService)
        {
            _entityStore = entityStore;
            _entityService = entityService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            return View(await GetViewModel(options));

        }

        async Task<EntityViewModel<Doc, DocComment>> GetViewModel(
            EntityOptions options)
        {

            if (options.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Id));
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(options.Id);

            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            // Populate previous and next entity
            await PopulatePreviousAndNextAsync(entity);

            // Populate child entities
            await PopulateChildEntitiesAsync(entity);

            // Return view model
            return new EntityViewModel<Doc, DocComment>
            {
                Options = options,
                Entity = entity
            };

        }

        async Task PopulatePreviousAndNextAsync(Doc entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get all other entities at the same level as our current entity
            var entities = await _entityService
                .ConfigureQuery(async q =>
                {

                    q.FeatureId.Equals(entity.FeatureId);
                    q.ParentId.Equals(entity.ParentId);
                    
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
                .GetResultsAsync();

            // Get the previous and next entities via the sort order
            if (entities != null)
            {
                entity.PreviousDoc = entities.Data?
                    .OrderByDescending(e => e.SortOrder)
                    .FirstOrDefault(e => e.SortOrder < entity.SortOrder); ;
                entity.NextDoc = entities.Data?
                    .OrderBy(e => e.SortOrder)
                    .FirstOrDefault(e => e.SortOrder > entity.SortOrder);
            }

        }

        async Task PopulateChildEntitiesAsync(Doc entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Get all child entities
            var entities = await _entityService
                .ConfigureQuery(async q =>
                {
                    q.ParentId.Equals(entity.Id);

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
                .GetResultsAsync(new EntityIndexOptions()
                {
                    Sort = SortBy.SortOrder,
                    Order = OrderBy.Asc
                });

            // Get the previous and next entities via the sort order
            entity.ChildEntities = entities?.Data;

        }

    }

}
