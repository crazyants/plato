using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
                return new ObjectResult(new
                {
                    follow,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Entity follow found successfully"
                });
            }

            return new ObjectResult(new
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = $"No entity follow with the Id {id} could be found"
            });

        }
        
        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityFollow follow)
        {
     
            // We need a user to subscribe to the entity
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return new ObjectResult(new
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Could not authenticate the request."
                });
            }

            // Is the user already following the entity?
            var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            if (existingFollow != null)
            {
                return new ObjectResult(new
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Authenticated user already following entity with id '{follow.EntityId}'"
                });
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
                return new ObjectResult(new
                {
                    result,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Entity follow created successfully"
                });
            }

            // We should not reach here
            return new ObjectResult(new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An error occurred whilst attempting to add the entity follow. It could be the user is already following the entity."
            });

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
                return new ObjectResult(new
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Could not authenticate your request"
                });
            }
            
            var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            if (existingFollow != null)
            {
                var success = await _entityFollowStore.DeleteAsync(existingFollow);
                if (success)
                {
                    return new ObjectResult(new
                    {
                        existingFollow,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Follow deleted successfully"
                    });
                }

            }

            return new ObjectResult(new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An error occurred whilst attempting to delete the entity follow"
            });


        }



    }
}
