using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reactions.Models;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Reactions.Controllers
{

    public class ReactController : BaseWebApiController
    {

        private readonly IEntityReactionsManager<EntityReaction> _entityReactionMAnager;
        private readonly ISimpleReactionsStore _simpleReactionsStore;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;

        public ReactController(
            ISimpleReactionsStore simpleReactionsStore,
            IEntityReactionsManager<EntityReaction> entityReactionMAnager,
            IEntityReactionsStore<EntityReaction> entityReactionsStore)
        {
            _simpleReactionsStore = simpleReactionsStore;
            _entityReactionMAnager = entityReactionMAnager;
            _entityReactionsStore = entityReactionsStore;
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
                foreach (var reaction in existingReactions.Where(r => r.EntityReplyId == model.EntityReplyId))
                {
                    if (reaction.ReactionName.Equals(model.ReactionName))
                    {
                        existingReaction = reaction;
                        break;
                    }
                }
            }

            // Delete any existing reaction
            if (existingReaction != null)
            {
                var delete = await _entityReactionMAnager.DeleteAsync(existingReaction);
                if (delete.Succeeded)
                {
                    // return 202 accepted to confirm delete
                    return base.AcceptedDelete(await _simpleReactionsStore.GetSimpleReactionsAsync(model.EntityId, model.EntityReplyId));
                }
            }
            
            // Set created by 
            model.CreatedUserId = user.Id;

            // Add and return results
            var result = await _entityReactionMAnager.CreateAsync(model);
            if (result.Succeeded)
            { 
                // return 201 created
                return base.Created(await _simpleReactionsStore.GetSimpleReactionsAsync(model.EntityId, model.EntityReplyId));
            }

            // We should not reach here
            return base.InternalServerError();

        }
        
    }

}
