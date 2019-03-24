using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Services;
using Plato.Entities.Ratings.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Net.Abstractions;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;
using Plato.Discuss.Models;

namespace Plato.Discuss.Votes.Controllers
{

    public class VoteController : BaseWebApiController
    {

        private readonly IEntityReplyRatingAggregator<Reply> _entityReplyRatingsAggregator;
        private readonly IEntityRatingAggregator<Topic> _entityRatingsAggregator;
        private readonly IEntityRatingsManager<EntityRating> _entityRatingManager;
        private readonly IEntityRatingsStore<EntityRating> _entityRatingsStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IClientIpAddress _clientIpAddress;

        public VoteController(
            IEntityReplyRatingAggregator<Reply> entityReplyRatingsAggregator,
            IEntityRatingAggregator<Topic> entityRatingsAggregator,
            IEntityRatingsManager<EntityRating> entityRatingManager,
            IEntityRatingsStore<EntityRating> entityRatingsStore,
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityStore<Topic> entityStore,
            IClientIpAddress clientIpAddress)
        {
            _entityReplyRatingsAggregator = entityReplyRatingsAggregator;
            _entityRatingsAggregator = entityRatingsAggregator;
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
                // Update reply
                if (model.EntityReplyId > 0)
                {
                    // return 202 accepted to confirm delete with updated response
                    return base.AcceptedDelete(await UpdateEntityReplyRating(model.EntityReplyId));
                }

                // Update entity
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

                // Update reply
                if (model.EntityReplyId > 0)
                {
                    // return 201 created
                    return base.Created(await UpdateEntityReplyRating(model.EntityReplyId));
                }
                
                // Update entity
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

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure we found the reply
            if (entity == null)
            {
                return null;
            }

            // Aggregate ratings
            var updatedEntity = await _entityRatingsAggregator.UpdateAsync(entity);
          
            // Return aggregated results
            return new AggregateRating()
            {
                TotalRatings = updatedEntity?.TotalRatings ?? 0,
                SummedRating = updatedEntity?.SummedRating ?? 0,
                MeanRating = updatedEntity?.MeanRating ?? 0,
                DailyRatings = updatedEntity?.DailyRatings ?? 0
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
            
            // Aggregate ratings
            var updatedReply = await _entityReplyRatingsAggregator.UpdateAsync(reply);

            // Return aggregated results
            return new AggregateRating()
            {
                TotalRatings = updatedReply?.TotalRatings ?? 0,
                SummedRating = updatedReply?.SummedRating ?? 0,
                MeanRating = updatedReply?.MeanRating ?? 0
            };

        }

    }

}
