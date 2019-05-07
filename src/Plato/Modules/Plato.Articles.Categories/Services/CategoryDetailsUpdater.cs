using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;

namespace Plato.Articles.Categories.Services
{
    
    public class CategoryDetailsUpdater : ICategoryDetailsUpdater
    {

        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<Comment> _replyStore;

        public CategoryDetailsUpdater(
            ICategoryStore<Category> categoryStore, 
            ICategoryManager<Category> categoryManager,
            IEntityStore<Article> entityStore, 
            IEntityReplyStore<Comment> replyStore)
        {
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
            _entityStore = entityStore;
            _replyStore = replyStore;
        }
        
        public async Task UpdateAsync(int categoryId)
        {
            
            // Get supplied category and all parent categories
            var parents = await _categoryStore.GetParentsByIdAsync(categoryId);

            // Update details within current and all parents
            foreach (var parent in parents)
            {
                
                // Get all children for current category
                var children = await _categoryStore.GetChildrenByIdAsync(parent.Id);

                // Get latest entity & total entity count for current category
                var entities = await _entityStore.QueryAsync()
                    .Take(1, 1) // we only need the latest entity
                    .Select<EntityQueryParams>(q =>
                    {

                        // If the category has children also include all child categories
                        if (children != null)
                        {
                            var categoryIds = children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray();
                            q.CategoryId.IsIn(categoryIds);
                        }
                        else
                        {
                            // Get entities for current categories
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("LastReplyDate", OrderBy.Desc)
                    .ToList();

                // Get latest reply & total reply count for current category
                var replies = await _replyStore.QueryAsync()
                    .Take(1, 1) // we only need the latest reply
                    .Select<EntityReplyQueryParams>(q =>
                    {

                        // If the category has children also include all child replies
                        if (children != null)
                        {
                            var categoryIds = children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray();
                            q.CategoryId.IsIn(categoryIds);
                        }
                        else
                        {
                            // Get entities for current category
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
                
                var totalTopics = 0;
                Article latestTopic = null;
                if (entities?.Data != null)
                {
                    totalTopics = entities.Total;
                    latestTopic = entities.Data[0];
                }

                var totalReplies = 0;
                Comment latestReply = null;
                if (replies?.Data != null)
                {
                    totalReplies = replies.Total;
                    latestReply = replies.Data[0];
                }

                // Update category details with latest entity details
                var details = parent.GetOrCreate<CategoryDetails>();
                details.TotalEntities = totalTopics;
                details.TotalReplies = totalReplies;

                if (latestTopic != null)
                {
                    details.LatestEntity = new LastPost
                    {
                        Id = latestTopic.Id,
                        Alias = latestTopic.Alias,
                        CreatedBy = latestTopic.CreatedBy,
                        CreatedDate = latestTopic.CreatedDate
                    };
                }
                else
                {
                    details.LatestEntity = null;
                }

                if (latestReply != null)
                {
                    details.LatestReply = new LastPost
                    {
                        Id = latestReply.Id,
                        CreatedBy = latestReply.CreatedBy,
                        CreatedDate = latestReply.CreatedDate
                    };
                }
                else
                {
                    details.LatestReply = null;
                }

                parent.AddOrUpdate<CategoryDetails>(details);

                // Save the updated details 
                await _categoryManager.UpdateAsync(parent);

            }
            
        }

    }

}
