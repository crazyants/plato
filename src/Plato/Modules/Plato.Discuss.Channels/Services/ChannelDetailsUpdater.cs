using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.Channels.Services
{
    
    public class ChannelDetailsUpdater : IChannelDetailsUpdater
    {

        private readonly ICategoryStore<Channel> _channelStore;
        private readonly ICategoryManager<Channel> _channelManager;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IEntityReplyStore<Reply> _replyStore;

        public ChannelDetailsUpdater(
            ICategoryStore<Channel> channelStore, 
            ICategoryManager<Channel> channelManager,
            IEntityStore<Topic> topicStore, 
            IEntityReplyStore<Reply> replyStore)
        {
            _channelStore = channelStore;
            _channelManager = channelManager;
            _topicStore = topicStore;
            _replyStore = replyStore;
        }
        
        public async Task UpdateAsync(int channelId)
        {
            
            // Get supplied channel and all parent channels
            var parents = await _channelStore.GetParentsByIdAsync(channelId);

            // Update details within current and all parents
            foreach (var parent in parents)
            {

                // Get latest topic & total topic count for current channel
                var topics = await _topicStore.QueryAsync()
                    .Take(1, 1) // we only need the latest topic
                    .Select<EntityQueryParams>(q =>
                    {
                        
                        // If the channel has children also include all child topics
                        if (parent.Children.Any())
                        {
                            q.CategoryId.IsIn(parent.Children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray());
                        }
                        else
                        {
                            // Get topics for current channel
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
                        if (parent.Children.Any())
                        {
                            q.CategoryId.IsIn(parent.Children
                                .Select(c => c.Id)
                                .Append(parent.Id)
                                .ToArray());
                        }
                        else
                        {
                            // Get topics for current channel
                            q.CategoryId.Equals(parent.Id);
                        }

                        q.HideSpam.True();
                        q.HidePrivate.True();
                        q.HideDeleted.True();
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
                
                var totalTopics = 0;
                Topic latestTopic = null;
                if (topics?.Data != null)
                {
                    totalTopics = topics.Total;
                    latestTopic = topics.Data[0];
                }

                var totalReplies = 0;
                Reply latestReply = null;
                if (replies?.Data != null)
                {
                    totalReplies = replies.Total;
                    latestReply = replies.Data[0];
                }

                // Update channel details with latest entity details
                var details = parent.GetOrCreate<ChannelDetails>();
                details.TotalTopics = totalTopics;
                details.TotalReplies = totalReplies;

                if (latestTopic != null)
                {
                    details.LastPost.EntityId = latestTopic.Id;
                    details.LastPost.CreatedBy = latestTopic.CreatedBy;
                    details.LastPost.CreatedDate = latestTopic.CreatedDate;
                }

                if (latestReply != null)
                {
                    details.LastPost.EntityReplyId = latestReply.Id;
                }

                parent.AddOrUpdate<ChannelDetails>(details);

                // Save the updated details 
                await _channelManager.UpdateAsync(parent);

            }
            
        }

    }

}
