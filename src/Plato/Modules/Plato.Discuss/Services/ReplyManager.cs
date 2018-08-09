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
            
            //_entityReplyManager.Created += async (sender, args) =>
            //{

            //    // Get last 5 participants

            //   var replies = await _entityReplyStore.QueryAsync()
            //       .Page(1, 5)
            //       .Select<EntityReplyQueryParams>(q =>
            //       {
            //           q.EntityId.Equals(args.Entity.Id);
            //       })
            //       .OrderBy("ModifiedDate", OrderBy.Desc)
            //       .ToList();

            //    var postDetails = args.Entity.TryGet<PostDetails>() ?? new PostDetails();
            //    postDetails.TotalReplies = postDetails.TotalReplies + 1;

            //    if (replies?.Data != null)
            //    {
            //        var participants = new List<EntityUser>();
            //        foreach (var reply in replies.Data)
            //        {
            //            participants.Add(reply.CreatedBy);
            //        }
            //        postDetails.Participants = participants;
            //    }

            //    args.Entity.AddOrUpdate<PostDetails>(postDetails);

            //    await _entityStore.UpdateAsync(args.Entity);

            //};

            var result =  await _entityReplyManager.CreateAsync(model);
            if (result.Succeeded)
            {

                var entity = await _entityStore.GetByIdAsync(result.Response.EntityId);
                if (entity == null)
                {
                    throw new Exception($"An entity for the reply could not be found!");
                }
                
                // Get last 20 public replies & total public reply count
                var replies = await _entityReplyStore.QueryAsync()
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
                
                // Get entity details to update
                var postDetails = entity.GetOrCreate<PostDetails>();

                // Update total public reply count
                postDetails.TotalReplies = replies.Total + 1;
                
                if (replies?.Data != null)
                {

                    const int max = 5;
                    var added = new List<int>();
                    var participants = new List<SimpleUser>();
                    foreach (var reply in replies.Data)
                    {
                        if (!added.Contains(reply.CreatedBy.Id))
                        {
                            added.Add(reply.CreatedBy.Id);
                            participants.Add(reply.CreatedBy);
                        }
                        if (added.Count >= max)
                        {
                            break;
                        }
                    }
                    postDetails.Participants = participants;
                    added.Clear();
                    participants.Clear();
                }

                entity.AddOrUpdate<PostDetails>(postDetails);

                await _entityStore.UpdateAsync(entity);
            }
            
            return result;

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
