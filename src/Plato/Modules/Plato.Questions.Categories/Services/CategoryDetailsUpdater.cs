using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Questions.Categories.Models;
using Plato.Questions.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;

namespace Plato.Questions.Categories.Services
{
    
    public class CategoryDetailsUpdater : ICategoryDetailsUpdater
    {

        private readonly ICategoryManager<Category> _channelManager;
        private readonly ICategoryStore<Category> _channelStore;
        private readonly IEntityReplyStore<Answer> _replyStore;
        private readonly IEntityStore<Question> _entityStore;

        public CategoryDetailsUpdater(
            ICategoryStore<Category> channelStore, 
            ICategoryManager<Category> channelManager,
            IEntityStore<Question> entityStore, 
            IEntityReplyStore<Answer> replyStore)
        {
            _channelStore = channelStore;
            _channelManager = channelManager;
            _entityStore = entityStore;
            _replyStore = replyStore;
        }
        
        public async Task UpdateAsync(int categoryId)
        {
            
            // Get supplied category and all parent categories
            var parents = await _channelStore.GetParentsByIdAsync(categoryId);

            // Update details within current and all parents
            foreach (var parent in parents)
            {
                
                // Get all children for current category
                var children = await _channelStore.GetChildrenByIdAsync(parent.Id);

                // Get latest topic & total topic count for current channel
                var entities = await _entityStore.QueryAsync()
                    .Take(1, 1) // we only need the latest topic
                    .Select<EntityQueryParams>(q =>
                    {

                        // Include entities from child channels?
                        if (children != null)
                        {
                            var channelIds = children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray();
                            q.CategoryId.IsIn(channelIds);
                        }
                        else
                        {
                            // Get topics for current channel
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("LastReplyDate", OrderBy.Desc)
                    .ToList();

                // Get latest reply & total reply count for current channel
                var replies = await _replyStore.QueryAsync()
                    .Take(1, 1) // we only need the latest reply
                    .Select<EntityReplyQueryParams>(q =>
                    {

                        // Include entities from child channels?
                        if (children != null)
                        {
                            var channelIds = children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray();
                            q.CategoryId.IsIn(channelIds);
                        }
                        else
                        {
                            // Get topics for current channel
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
                
                var totalEntities = 0;
                Question latestEntity = null;
                if (entities?.Data != null)
                {
                    totalEntities = entities.Total;
                    latestEntity = entities.Data[0];
                }

                var totalReplies = 0;
                Answer latestReply = null;
                if (replies?.Data != null)
                {
                    totalReplies = replies.Total;
                    latestReply = replies.Data[0];
                }

                // Update channel details with latest entity details
                var details = parent.GetOrCreate<CategoryDetails>();
                details.TotalEntities = totalEntities;
                details.TotalReplies = totalReplies;

                if (latestEntity != null)
                {
                    details.LatestEntity = new LatestPost
                    {
                        Id = latestEntity.Id,
                        Alias = latestEntity.Alias,
                        CreatedBy = latestEntity.CreatedBy,
                        CreatedDate = latestEntity.CreatedDate
                    };
                }
                else
                {
                    details.LatestEntity = null;
                }

                if (latestReply != null)
                {
                    details.LatestReply = new LatestPost
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
                await _channelManager.UpdateAsync(parent);

            }
            
        }

    }

}
