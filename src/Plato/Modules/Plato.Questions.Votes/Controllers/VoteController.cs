using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Services;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Net.Abstractions;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.Questions.Models;

namespace Plato.Questions.Votes.Controllers
{

    public class VoteController : BaseWebApiController
    {

        private readonly IEntityRatingsAggregateStore _entityRatingsAggregateStore;
        private readonly IEntityRatingsManager<EntityRating> _entityRatingManager;
        private readonly IEntityRatingsStore<EntityRating> _entityRatingsStore;
        private readonly IEntityReplyStore<Answer> _entityReplyStore;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IClientIpAddress _clientIpAddress;

        public VoteController(
            IEntityRatingsAggregateStore entityRatingsAggregateStore,
            IEntityRatingsManager<EntityRating> entityRatingManager,
            IEntityRatingsStore<EntityRating> entityRatingsStore,
            IEntityReplyStore<Answer> entityReplyStore,
            IEntityStore<Question> entityStore,
            IClientIpAddress clientIpAddress)
        {
            _entityRatingsAggregateStore = entityRatingsAggregateStore;
            _entityRatingManager = entityRatingManager;
            _entityRatingsStore = entityRatingsStore;
            _clientIpAddress = clientIpAddress;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityRating model)
        {
            
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Delete all existing votes, We can only vote once
            var deleted = false;
            var existingRatings = await _entityRatingsStore.SelectEntityRatingsByUserIdAndEntityId(user.Id, model.EntityId);
            if (existingRatings != null)
            {
                foreach (var rating in existingRatings.Where(r => r.EntityReplyId == model.EntityReplyId))
                {
                    var delete = await _entityRatingManager.DeleteAsync(rating);
                    if (delete.Succeeded)
                    {
                        deleted = true;
                    }
                }
            }
            
            if (deleted)
            {

                if (model.EntityReplyId > 0)
                {
                    // return 202 accepted to confirm delete with updated response
                    return base.AcceptedDelete(await UpdateEntityReplyRating(model.EntityReplyId));
                }
                
                if (model.EntityId > 0)
                {
                    // return 202 accepted to confirm delete with updated response
                    return base.AcceptedDelete(await UpdateEntityRating(model.EntityId));
                }

            }
            
            // Set created by 
            model.CreatedUserId = user.Id;
            model.CreatedDate = DateTimeOffset.UtcNow;
            model.IpV4Address = _clientIpAddress.GetIpV4Address();
            model.IpV6Address = _clientIpAddress.GetIpV6Address();
            if (Request.Headers.ContainsKey("User-Agent"))
            {
                model.UserAgent = Request.Headers["User-Agent"].ToString();
            }
            
            // Add and return results
            var result = await _entityRatingManager.CreateAsync(model);
            if (result.Succeeded)
            {

                if (model.EntityReplyId > 0)
                {
                    // return 201 created
                    return base.Created(await UpdateEntityReplyRating(model.EntityReplyId));

                }
                
                if (model.EntityId > 0)
                {
                    // return 201 created
                    return base.Created(await UpdateEntityRating(model.EntityId));
                }

            }

            // We should not reach here
            return base.InternalServerError();

        }


        async Task<AggregateRating> UpdateEntityRating(int entityId)
        {

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure we found the reply
            if (entity == null)
            {
                return null;
            }
            
            // Get aggregate rating
            var aggregateRating = await _entityRatingsAggregateStore.SelectAggregateRating(entity.Id);

            // Update entity
            entity.TotalRatings = aggregateRating?.TotalRatings ?? 0;
            entity.MeanRating = aggregateRating?.MeanRating ?? 0;
            entity.DailyRatings = aggregateRating?.DailyRatings ?? 0;

            // Get updated entity
            var updatedEntity = await _entityStore.GetByIdAsync(entity.Id);

            return new AggregateRating()
            {
                TotalRatings = entity?.TotalRatings ?? 0,
                MeanRating = entity?.MeanRating ?? 0,
                DailyRatings = entity?.DailyRatings ?? 0
            };

        }

        async Task<AggregateRating> UpdateEntityReplyRating(int replyId)
        {

            // Get reply
            var reply = await _entityReplyStore.GetByIdAsync(replyId);

            // Ensure we found the reply
            if (reply == null)
            {
                return null;
            }

            // Get aggregate rating
            var aggregateRating = await _entityRatingsAggregateStore.SelectAggregateRating(reply.EntityId, reply.Id);

            // Update entity
            reply.TotalRatings = aggregateRating?.TotalRatings ?? 0;
            reply.MeanRating = aggregateRating?.MeanRating ?? 0;

            // Persist label updates
            var updatedReply = await _entityReplyStore.UpdateAsync(reply);

            return new AggregateRating()
            {
                TotalRatings = updatedReply?.TotalRatings ?? 0,
                MeanRating = updatedReply?.MeanRating ?? 0
            };

        }

    }

}
