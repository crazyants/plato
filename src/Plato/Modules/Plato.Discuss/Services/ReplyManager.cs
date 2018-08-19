using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Services
{

    public class ReplyManager : IPostManager<Reply>
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyManager<Reply> _entityReplyManager;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        public ReplyManager(
            IEntityStore<Topic> entityStore,
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityReplyManager<Reply> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public async Task<IActivityResult<Reply>> CreateAsync(Reply model)
        {

            _entityReplyManager.Created += async (sender, args) =>
            {


            };

            var result =  await _entityReplyManager.CreateAsync(model);
            if (result.Succeeded)
            {

                var entity = await _entityStore.GetByIdAsync(result.Response.EntityId);
                if (entity == null)
                {
                    throw new Exception($"An entity for the reply could not be found!");
                }

                // Increment reply count
                entity.TotalReplies = entity.TotalReplies + 1;

                // Get entity details to update
                var details = entity.GetOrCreate<PostDetails>();
                
                // Get last 20 public replies & total public reply count
                var replies = await GetLatestReplies(result);
                if (replies?.Data != null)
                {

                    const int max = 5;
                    var added = new List<int>();
                    var simpleReplies = new List<SimpleReply>();
                    foreach (var reply in replies.Data)
                    {
                        if (!added.Contains(reply.Id))
                        {
                            added.Add(reply.Id);
                            simpleReplies.Add(new SimpleReply()
                            {
                                Id = reply.Id,
                                CreatedBy = reply.CreatedBy,
                                CreatedDate = reply.CreatedDate
                            });
                        }
                        if (added.Count >= max)
                        {
                            break;
                        }
                    }
                    details.LatestReplies = simpleReplies;
                }

                details.LatestReply.Id = result.Response.Id;
                details.LatestReply.CreatedBy = result.Response.CreatedBy;
                details.LatestReply.CreatedDate = result.Response.CreatedDate;

                // Add updated data to entity
                entity.AddOrUpdate<PostDetails>(details);

                // Persist the updates
                await _entityStore.UpdateAsync(entity);


            }

            return result;

        }

        private async Task<IPagedResults<Reply>> GetLatestReplies(IActivityResult<Reply> result)
        {
            return await _entityReplyStore.QueryAsync()
                .Take(1, 20)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(result.Response.EntityId);
                    q.IsPrivate.False();
                    q.IsSpam.False();
                    q.IsDeleted.False();
                })
                .OrderBy("ModifiedDate", OrderBy.Desc)
                .ToList();
        }

        public async Task<IActivityResult<Reply>> UpdateAsync(Reply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {

            };
            
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<IActivityResult<Reply>> DeleteAsync(Reply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {

            };

            return await _entityReplyManager.DeleteAsync(model.Id);
            
        }

        #endregion

        #region "Private Methods"

        #endregion


    }
}
