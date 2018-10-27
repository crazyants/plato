using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Plato.Reactions.Models;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Reactions.Controllers
{

    public class ReactionController : BaseWebApiController
    {

        private readonly IEntityReactionsManager<EntityReaction> _entityReactionMAnager;
        private readonly IReactionsStore<Reaction> _reactionsStore;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;

        public ReactionController(
            IReactionsStore<Reaction> reactionsStore,
            IEntityReactionsManager<EntityReaction> entityReactionMAnager,
            IEntityReactionsStore<EntityReaction> entityReactionsStore)
        {
            _reactionsStore = reactionsStore;
            _entityReactionMAnager = entityReactionMAnager;
            _entityReactionsStore = entityReactionsStore;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int entityId, int entityReplyId)
        {

            var data = await _reactionsStore.GetEntityReactionsGroupedAsync(entityId, entityReplyId);
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
            
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Is the user already following the entity?
            EntityReaction existingReaction = null;
            var existingReactions = await _entityReactionsStore.SelectEntityReactionsByUserIdAndEntityId(user.Id, model.EntityId);
            if (existingReactions != null)
            {
                foreach (var reaction in existingReactions)
                {
                    if (reaction.ReactionName.Equals(model.ReactionName))
                    {
                        existingReaction = reaction;
                    }
                }
            }

            // Delete any existing reaction
            if (existingReaction != null)
            {
                var delete = await _entityReactionMAnager.DeleteAsync(existingReaction);
                if (delete.Succeeded)
                {
                    return base.Created(await _reactionsStore.GetEntityReactionsGroupedAsync(model.EntityId, model.EntityReplyId));
                }
            }
            
            // Set created by 
            model.CreatedUserId = user.Id;

            // Add and return result
            var result = await _entityReactionMAnager.CreateAsync(model);
            if (result.Succeeded)
            {
                return base.Created(await _reactionsStore.GetEntityReactionsGroupedAsync(model.EntityId, model.EntityReplyId));
            }

            // We should not reach here
            return base.InternalServerError();

        }





    }

}
