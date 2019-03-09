using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Entities.Reactions.Stores;
using Plato.Internal.Net.Abstractions;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Entities.Reactions.Controllers
{

    public class ReactController : BaseWebApiController
    {
        
        private readonly IEntityReactionsManager<EntityReaction> _entityReactionManager;
        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;
        private readonly ISimpleReactionsStore _simpleReactionsStore;
        private readonly IClientIpAddress _clientIpAddress;

        public ReactController(
            IEntityReactionsManager<EntityReaction> entityReactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionsStore,
            ISimpleReactionsStore simpleReactionsStore,
            IClientIpAddress clientIpAddress)
        {
            _simpleReactionsStore = simpleReactionsStore;
            _entityReactionManager = entityReactionManager;
            _entityReactionsStore = entityReactionsStore;
            _clientIpAddress = clientIpAddress;
        }

        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityReaction model)
        {
            
            var user = await base.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return base.UnauthorizedException();
            }

            // Has the user already reacted to the entity?
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
                var delete = await _entityReactionManager.DeleteAsync(existingReaction);
                if (delete.Succeeded)
                {
                    // return 202 accepted to confirm delete
                    return base.AcceptedDelete(await _simpleReactionsStore.GetSimpleReactionsAsync(model.EntityId, model.EntityReplyId));
                }
            }
            
            // Set created by 
            model.CreatedUserId = user.Id;
            model.CreatedDate = DateTimeOffset.UtcNow;
            model.IpV4Address = _clientIpAddress.GetIpV4Address();
            model.IpV6Address = _clientIpAddress.GetIpV6Address();
            if (Request.Headers.ContainsKey("User-Agent"))
            {
                model.UserAgent = Request.Headers["User-Agent"].ToString();
            }
            
            // Add and return results
            var result = await _entityReactionManager.CreateAsync(model);
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
