using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Follows.Models;
using Plato.Follows.Services;
using Plato.Follows.Stores;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Follows.Controllers
{

    public class FollowController : BaseWebApiController
    {

        private readonly IFollowStore<Models.Follow> _followStore;
        private readonly IFollowManager<Models.Follow> _followManager;

        public FollowController(
            IFollowStore<Models.Follow> followStore,
            IFollowManager<Models.Follow> followManager)
        {
            _followStore = followStore;
            _followManager = followManager;
        }

        [HttpGet, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int id)
        {
            var follow = await _followStore.GetByIdAsync(id);
            if (follow != null)
            {
                return base.Result(follow);
            }
            return base.NotFound();
        }
        
        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] Models.Follow follow)
        {
     
            // We need a user to subscribe to the thing
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Is the user already following the thing?
            var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                follow.Name,
                follow.ThingId,
                user.Id);
            if (existingFollow != null)
            {
                return base.Result(HttpStatusCode.OK,
                    $"Authenticated user already following object with id '{follow.ThingId}'");
            }

            // Build a new subscription
            var followToAdd = new Models.Follow()
            {
                Name = follow.Name,
                ThingId = follow.ThingId,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            // Add and return result
            var result = await _followManager.CreateAsync(followToAdd);
            if (result != null)
            {
                return base.Result(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpPut, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(Models.Follow follow)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete([FromBody] Models.Follow follow)
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                follow.Name,
                follow.ThingId,
                user.Id);
            if (existingFollow != null)
            {
                var result = await _followManager.DeleteAsync(existingFollow);
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
