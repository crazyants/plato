using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Services
{

    public class ReplyManager : IPostManager<EntityReply>
    {

        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyManager<EntityReply> _entityReplyManager;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;

        public ReplyManager(
            IEntityReplyManager<EntityReply> entityReplyManager,
            IEntityReplyStore<EntityReply> entityReplyStore,
            IEntityStore<Entity> entityStore)
        {
            _entityReplyManager = entityReplyManager;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        public async Task<IActivityResult<EntityReply>> CreateAsync(EntityReply model)
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

                var postDetails = entity.GetOrCreate<PostDetails>();
                postDetails.TotalReplies = (postDetails.TotalReplies + 1);

                // Get last 5 participants

                var replies = await _entityReplyStore.QueryAsync()
                    .Take(1, 5)
                    .Select<EntityReplyQueryParams>(q =>
                    {
                        q.EntityId.Equals(result.Response.EntityId);
                    })
                    .OrderBy("ModifiedDate", OrderBy.Desc)
                    .ToList();

                if (replies?.Data != null)
                {
                    var participants = new List<SimpleUser>();
                    foreach (var reply in replies.Data)
                    {
                        participants.Add(reply.CreatedBy);
                    }
                    postDetails.Participants = participants;
                }

                entity.AddOrUpdate<PostDetails>(postDetails);

                await _entityStore.UpdateAsync(entity);
            }
            
            return result;

        }

        public async Task<IActivityResult<EntityReply>> UpdateAsync(EntityReply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {

            };
            
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        

        public async Task<IActivityResult<EntityReply>> DeleteAsync(EntityReply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {

            };

            return await _entityReplyManager.DeleteAsync(model.Id);
            
        }


    }
}
