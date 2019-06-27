using System;
using System.Threading.Tasks;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Tags.Services
{

    public class TagOccurrencesUpdater<TModel> : ITagOccurrencesUpdater<TModel> where TModel: class, ITag
    {

        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly ITagManager<TModel> _tagManager;

        public TagOccurrencesUpdater(
            IEntityTagStore<EntityTag> entityTagStore,
            ITagManager<TModel> tagManager)
        {
            _entityTagStore = entityTagStore;
            _tagManager = tagManager;
        }

        public async Task UpdateAsync(TModel tag)
        {

            // Get count for public entities & replies tagged with this tag
            var entityTags = await _entityTagStore.QueryAsync()
                .Take(1)
                .Select<EntityTagQueryParams>(q =>
                {
                    q.TagId.Equals(tag.Id);
                    q.HideHidden.True();
                    q.HidePrivate.True();
                    q.HideSpam.True();
                    q.HideDeleted.True();
                })
                .ToList();

            // Update 
            tag.TotalEntities = entityTags?.Total ?? 0;
        
            // Persist 
            await _tagManager.UpdateAsync(tag);

        }

    }

}
