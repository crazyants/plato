using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Stars.Models;
using Plato.Stars.Services;
using Plato.Stars.Stores;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Stars.Controllers
{

    public class StarController : BaseWebApiController
    {

        private readonly IStarStore<Star> _starStore;
        private readonly IStarManager<Star> _starManager;

        public StarController(
            IStarStore<Star> starStore,
            IStarManager<Star> starManager)
        {
            _starStore = starStore;
            _starManager = starManager;
        }

        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int id)
        {
            var follow = await _starStore.GetByIdAsync(id);
            if (follow != null)
            {
                return base.Result(follow);
            }
            return base.NotFound();
        }
        
        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] Star follow)
        {
     
            // We need a user to subscribe to the thing
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Has the user already starred the thing?
            var existingStar = await _starStore.SelectByNameThingIdAndCreatedUserId(
                follow.Name,
                follow.ThingId,
                user.Id);
            if (existingStar != null)
            {
                return base.Result(HttpStatusCode.OK,
                    $"Authenticated user already stared object with id '{follow.ThingId}'");
            }

            // Build a new subscription
            var starToAdd = new Star()
            {
                Name = follow.Name,
                ThingId = follow.ThingId,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            // Add and return result
            var result = await _starManager.CreateAsync(starToAdd);
            if (result != null)
            {
                // Award reputation
              
                return base.Result(result);

            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete([FromBody] Star star)
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            var existingStar = await _starStore.SelectByNameThingIdAndCreatedUserId(
                star.Name,
                star.ThingId,
                user.Id);
            if (existingStar != null)
            {
                var result = await _starManager.DeleteAsync(existingStar);
                if (result.Succeeded)
                {
                    return base.Result(existingStar);
                }

            }

            // We should not reach here
            return base.InternalServerError();

        }
        
    }

}
