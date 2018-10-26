using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Abstractions.Extensions;
using Plato.Reactions.Models;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Reactions.Controllers
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

            //// Is the user already following the entity?
            //EntityReaction existingReaction = null;
            //var existingReactions = await _entityReactionsStore.SelectEntityReactionsByUserIdAndEntityId(user.Id, model.EntityId);
            //if (existingReactions != null)
            //{
            //    foreach (var reaction in existingReactions)
            //    {
            //        if (reaction.ReactionName.Equals(model.ReactionName))
            //        {
            //            existingReaction = reaction;
            //        }
            //    }
            //}
            
            // Add and return result
            var result = await _entityReactionMAnager.CreateAsync(new EntityReaction()
            {
                ReactionName = model.ReactionName,
                EntityId = model.EntityId,
                CreatedUserId = user.Id
            });
            if (result.Succeeded)
            {
                return base.Created(result);
            }

            // We should not reach here
            return base.InternalServerError();

        }

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(EntityReaction model)
        {
            throw new NotImplementedException();
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
