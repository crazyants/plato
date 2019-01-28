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

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int id)
        {
            var follow = await _starStore.GetByIdAsync(id);
            if (follow != null)
            {
                return base.Result(follow);
            }
            return base.NotFound();
        }
        
        [HttpPost, ValidateClientAntiForgeryToken]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] Star follow)
        {
     
            // We need a user to subscribe to the thing
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Is the user already following the thing?
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
            var followToAdd = new Star()
            {
                Name = follow.Name,
                ThingId = follow.ThingId,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            // Add and return result
            var result = await _starManager.CreateAsync(followToAdd);
            if (result != null)
            {

                // Award reputation
                
                // Send notifications
                
                return base.Result(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(Star follow)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete([FromBody] Star follow)
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            var existingFollow = await _starStore.SelectByNameThingIdAndCreatedUserId(
                follow.Name,
                follow.ThingId,
                user.Id);
            if (existingFollow != null)
            {
                var result = await _starManager.DeleteAsync(existingFollow);
                if (result.Succeeded)
                {
                    return base.Result(existingFollow);
                }

            }

            // We should not reach here
            return base.InternalServerError();

        }
        
    }

}
