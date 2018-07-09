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

   
            var user = await base.GetAuthenticatedUserAsync();

            return new ObjectResult(new
            {
                StatusCode = HttpStatusCode.OK,
                Message = user != null ? user.UserName : "no user found"
            });
            
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

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(EntityFollow follow)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post(EntityFollow follow)
        {
            
            // We need a user to subscribe to the entity
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return new ObjectResult(new
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Could not authenticate your request"
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
                CancellationGuid = "123456",
                CreatedDate = DateTime.UtcNow
            };

            // Add and return result
            var result = _entityFollowStore.CreateAsync(followToAdd);
            if (result != null)
            {
                return new ObjectResult(new
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Entity follow created successfully"
                });
            }

            // We should not reach here
            return new ObjectResult(new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An error occurred whilst attempting to add the entity follow"
            });


        }

        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete(int entityId)
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
            
            var follow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, entityId);
            if (follow != null)
            {
                var success = await _entityFollowStore.DeleteAsync(follow);
                if (success)
                {
                    return new ObjectResult(new
                    {
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
