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
    
    public class ChannelDetailsUpdater : IChannelDetailsUpdater
    {

        private readonly ICategoryStore<Category> _channelStore;
        private readonly ICategoryManager<Category> _channelManager;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<Comment> _replyStore;

        public ChannelDetailsUpdater(
            ICategoryStore<Category> channelStore, 
            ICategoryManager<Category> channelManager,
            IEntityStore<Article> entityStore, 
            IEntityReplyStore<Comment> replyStore)
        {
            _channelStore = channelStore;
            _channelManager = channelManager;
            _entityStore = entityStore;
            _replyStore = replyStore;
        }
        
        public async Task UpdateAsync(int channelId)
        {
            
            // Get supplied channel and all parent channels
            var parents = await _channelStore.GetParentsByIdAsync(channelId);

            // Update details within current and all parents
            foreach (var parent in parents)
            {
                
                // Get all children for current channel
                var children = await _channelStore.GetChildrenByIdAsync(parent.Id);

                // Get latest entity & total entity count for current category
                var entities = await _entityStore.QueryAsync()
                    .Take(1, 1) // we only need the latest entity
                    .Select<EntityQueryParams>(q =>
                    {

                        // If the category has children also include all child categories
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
                            // Get entities for current channel
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HidePrivate.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("LastReplyDate", OrderBy.Desc)
                    .ToList();

                // Get latest reply & total reply count for current channel
                var replies = await _replyStore.QueryAsync()
                    .Take(1, 1) // we only need the latest reply
                    .Select<EntityReplyQueryParams>(q =>
                    {

                        // If the channel has children also include all child replies
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
                            // Get entities for current channel
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HidePrivate.True();
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

                // Update channel details with latest entity details
                var details = parent.GetOrCreate<CategoryDetails>();
                details.TotalTopics = totalTopics;
                details.TotalReplies = totalReplies;

                if (latestTopic != null)
                {
                    details.LastTopic.Id = latestTopic.Id;
                    details.LastTopic.Alias = latestTopic.Alias;
                    details.LastTopic.CreatedBy = latestTopic.CreatedBy;
                    details.LastTopic.CreatedDate = latestTopic.CreatedDate;
                }

                if (latestReply != null)
                {
                    details.LastReply.Id = latestReply.Id;
                    details.LastReply.CreatedBy = latestReply.CreatedBy;
                    details.LastReply.CreatedDate = latestReply.CreatedDate;
                }

                parent.AddOrUpdate<CategoryDetails>(details);

                // Save the updated details 
                await _channelManager.UpdateAsync(parent);

            }
            
        }

    }

}
