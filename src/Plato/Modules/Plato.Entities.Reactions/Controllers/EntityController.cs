using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Entities.Reactions.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Entities.Reactions.Controllers
{

    public class EntityController : BaseWebApiController
    {

        private readonly IEntityReactionsManager<EntityReaction> _entityReactionMAnager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;

        public EntityController(
            IEntityReactionsStore<EntityReaction> entityReactionsStore,
            IEntityReactionsManager<EntityReaction> entityReactionMAnager)
        {
            _entityReactionsStore = entityReactionsStore;
            _entityReactionMAnager = entityReactionMAnager;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _entityReactionsStore.GetByIdAsync(id);
            if (data != null)
            {
                return base.Result(data);
            }
            return base.NotFound();
        }
        

        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityReaction model)
        {
            
            // We need a user to subscribe to the entity
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Add and return result
            var result = await _entityReactionMAnager.CreateAsync(model);
            if (result.Succeeded)
            {
                return base.Created(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Put(EntityReaction model)
        {
  
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            var result = await _entityReactionMAnager.UpdateAsync(model);
            if (result.Succeeded)
            {
                return base.Created(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Delete([FromBody] EntityReaction model)
        {

            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }
            
            var existingReaction = await _entityReactionsStore.GetByIdAsync(model.Id);
            if (existingReaction != null)
            {
                
                var success = await _entityReactionsStore.DeleteAsync(existingReaction);
                if (success)
                {
                    return base.Result(existingReaction);
                }

            }

            // We should not reach here
            return base.InternalServerError();

        }



    }
}
