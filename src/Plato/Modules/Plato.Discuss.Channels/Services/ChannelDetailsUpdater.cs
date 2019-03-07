using System.Linq;
using System.Threading.Tasks;
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

        private readonly ICategoryStore<ChannelHome> _channelStore;
        private readonly ICategoryManager<ChannelHome> _channelManager;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IEntityReplyStore<Reply> _replyStore;

        public ChannelDetailsUpdater(
            ICategoryStore<ChannelHome> channelStore, 
            ICategoryManager<ChannelHome> channelManager,
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
                
                // Get all children for current channel
                var children = await _channelStore.GetChildrenByIdAsync(parent.Id);

                // Get latest topic & total topic count for current channel
                var topics = await _topicStore.QueryAsync()
                    .Take(1, 1) // we only need the latest topic
                    .Select<EntityQueryParams>(q =>
                    {

                        // If the channel has children also include all child topics
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

                parent.AddOrUpdate<ChannelDetails>(details);

                // Save the updated details 
                await _channelManager.UpdateAsync(parent);

            }
            
        }

    }

}
