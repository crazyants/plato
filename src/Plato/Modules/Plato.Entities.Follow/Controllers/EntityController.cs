using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Follow.Models;
using Plato.Entities.Follow.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Entities.Follow.Controllers
{

    public class EntityController : BaseWebApiController
    {

        private readonly IEntityFollowStore<EntityFollow> _entityFollowStore;

        public EntityController(
            IEntityFollowStore<EntityFollow> entityFollowStore)
        {
            _entityFollowStore = entityFollowStore;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int id)
        {
            var follow = await _entityFollowStore.GetByIdAsync(id);
            if (follow != null)
            {
                return base.Result(follow);
            }
            return base.NotFound();
        }
        
        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityFollow follow)
        {
     
            // We need a user to subscribe to the entity
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Is the user already following the entity?
            var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            if (existingFollow != null)
            {
                return base.Result(HttpStatusCode.OK,
                    $"Authenticated user already following entity with id '{follow.EntityId}'");
            }

            // Build a new subscription
            var followToAdd = new EntityFollow()
            {
                EntityId = follow.EntityId,
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            // Add and return result
            var result = await _entityFollowStore.CreateAsync(followToAdd);
            if (result != null)
            {
                return base.Result(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(EntityFollow follow)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete([FromBody] EntityFollow follow)
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            if (existingFollow != null)
            {
                var success = await _entityFollowStore.DeleteAsync(existingFollow);
                if (success)
                {
                    return base.Result(existingFollow);
                }

            }

            // We should not reach here
            return base.InternalServerError();

        }



    }
}
