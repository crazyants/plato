using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocViewComponent : ViewComponent
    {

        private readonly IEntityService<Doc> _entityService;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IEntityReplyStore<DocComment> _entityReplyStore;

        public DocViewComponent(
            IEntityReplyStore<DocComment> entityReplyStore,
            IEntityStore<Doc> entityStore,
            IEntityService<Doc> entityService)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _entityService = entityService;
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
            
            // Get all other entities at the same level as our current entity
            var entities = await _entityService
                .ConfigureQuery(q =>
                {
                    q.FeatureId.Equals(entity.FeatureId);
                    q.ParentId.Equals(entity.ParentId);
                })
                .GetResultsAsync(new EntityIndexOptions()
                {
                    Sort = SortBy.SortOrder,
                    Order = OrderBy.Asc
                });

            // Get the previous and next entities via the sort order
            if (entities != null)
            {
                entity.PreviousDoc = entities.Data?.FirstOrDefault(e => e.SortOrder < entity.SortOrder); ;
                entity.NextDoc = entities.Data?.FirstOrDefault(e => e.SortOrder > entity.SortOrder);
            }
            
            // Return view model
            return new EntityViewModel<Doc, DocComment>
            {
                Options = options,
                Entity = entity
            };

        }

    }

}
