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

namespace Plato.Entities.Ratings.Controllers
{

    public class RateController : BaseWebApiController
    {


        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityRatingsManager<EntityRating> _entityRatingManager;
        private readonly IEntityRatingsStore<EntityRating> _entityRatingsStore;
        private readonly IClientIpAddress _clientIpAddress;

        public RateController(
            IEntityRatingsManager<EntityRating> entityRatingManager,
            IEntityRatingsStore<EntityRating> entityRatingsStore,
            IClientIpAddress clientIpAddress, 
            IEntityStore<Entity> entityStore)
        {
     
            _entityRatingManager = entityRatingManager;
            _entityRatingsStore = entityRatingsStore;
            _clientIpAddress = clientIpAddress;
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

            // Has the user already rated to the entity?
            EntityRating existingRating = null;
            var existingRatings = await _entityRatingsStore.SelectEntityRatingsByUserIdAndEntityId(user.Id, model.EntityId);
            if (existingRatings != null)
            {
                foreach (var rating in existingRatings.Where(r => r.EntityReplyId == model.EntityReplyId))
                {
                    if (model.Rating == rating.Rating)
                    {
                        existingRating = rating;
                    }
                    
                    break;
                }
            }

            // Delete any existing rating
            if (existingRating != null)
            {
                var delete = await _entityRatingManager.DeleteAsync(existingRating);
                if (delete.Succeeded)
                {

                    // Get updated entity
                    var entity = await _entityStore.GetByIdAsync(existingRating.EntityId);

                    // return 202 accepted to confirm delete with updated response
                    return base.AcceptedDelete(new AggregateRating()
                    {
                        TotalRatings = entity.TotalRatings,
                        MeanRating = entity.MeanRating,
                        DailyRatings = entity.DailyRatings
                    });

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

                var entity = await _entityStore.GetByIdAsync(result.Response.EntityId);

                // return 201 created
                return base.Created(new AggregateRating()
                {
                    TotalRatings = entity.TotalRatings,
                    MeanRating = entity.MeanRating,
                    DailyRatings = entity.DailyRatings
                });
            }

            // We should not reach here
            return base.InternalServerError();

        }
        
    }

}
