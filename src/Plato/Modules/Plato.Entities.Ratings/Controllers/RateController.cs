using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Ratings.Models;
using Plato.Entities.Ratings.Services;
using Plato.Entities.Ratings.Stores;
using Plato.Internal.Net.Abstractions;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Entities.Ratings.Controllers
{

    public class RateController : BaseWebApiController
    {
        
        private readonly IEntityRatingsManager<EntityRating> _entityRatingManager;
        private readonly IEntityRatingsStore<EntityRating> _entityRatingsStore;
        private readonly ISimpleRatingsStore _simpleRatingsStore;
        private readonly IClientIpAddress _clientIpAddress;

        public RateController(
            IEntityRatingsManager<EntityRating> entityRatingManager,
            IEntityRatingsStore<EntityRating> entityRatingsStore,
            ISimpleRatingsStore simpleRatingsStore,
            IClientIpAddress clientIpAddress)
        {
            _simpleRatingsStore = simpleRatingsStore;
            _entityRatingManager = entityRatingManager;
            _entityRatingsStore = entityRatingsStore;
            _clientIpAddress = clientIpAddress;
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
                    existingRating = rating;
                    break;
                }
            }

            // Delete any existing rating
            if (existingRating != null)
            {
                var delete = await _entityRatingManager.DeleteAsync(existingRating);
                if (delete.Succeeded)
                {
                    // return 202 accepted to confirm delete
                    return base.AcceptedDelete(await _simpleRatingsStore.GetSimpleRatingsAsync(model.EntityId, model.EntityReplyId));
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
                // return 201 created
                return base.Created(await _simpleRatingsStore.GetSimpleRatingsAsync(model.EntityId, model.EntityReplyId));
            }

            // We should not reach here
            return base.InternalServerError();

        }
        
    }

}
