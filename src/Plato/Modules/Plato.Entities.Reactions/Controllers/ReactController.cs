using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Entities.Reactions.Stores;
using Plato.Internal.Data.Abstractions;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IClientIpAddress _clientIpAddress;

        public ReactController(
            IEntityReactionsManager<EntityReaction> entityReactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionsStore,
            ISimpleReactionsStore simpleReactionsStore,
            IAuthorizationService authorizationService,
            IClientIpAddress clientIpAddress)
        {
            _simpleReactionsStore = simpleReactionsStore;
            _entityReactionManager = entityReactionManager;
            _entityReactionsStore = entityReactionsStore;
            _clientIpAddress = clientIpAddress;
            _authorizationService = authorizationService;
        }

        [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] EntityReaction model)
        {

            // Get authenticated user (if any)
            var user = await base.GetAuthenticatedUserAsync();
            
            // Get client details
            var ipV4Address = _clientIpAddress.GetIpV4Address();
            var ipV6Address = _clientIpAddress.GetIpV6Address();
            var userAgent = "";
            if (Request.Headers.ContainsKey("User-Agent"))
            {
                userAgent = Request.Headers["User-Agent"].ToString();
            }
            
            IEnumerable<EntityReaction> existingReactions = null;
            if (user != null)
            {
                // Has the user already reacted to the entity?
                existingReactions = await _entityReactionsStore.SelectEntityReactionsByUserIdAndEntityId(user.Id, model.EntityId);
            }
            else
            {
                // Delete all existing reactions for anonymous user using there
                // IP addresses & user agent as a unique identifier 
                var anonymousReactions = await _entityReactionsStore.QueryAsync()
                    .Select<EntityReactionsQueryParams>(q =>
                    {
                        q.EntityId.Equals(model.EntityId);
                        q.CreatedUserId.Equals(0);
                        q.IpV4Address.Equals(ipV4Address);
                        q.IpV6Address.Equals(ipV6Address);
                        q.UserAgent.Equals(userAgent);
                    })
                    .ToList();
                existingReactions = anonymousReactions?.Data;
            }

            // Get reaction from existing reactions
            EntityReaction existingReaction = null;
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
            model.CreatedUserId = user?.Id ?? 0;
            model.CreatedDate = DateTimeOffset.UtcNow;
            model.IpV4Address = ipV4Address;
            model.IpV6Address = ipV6Address;
            model.UserAgent = userAgent;
            
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
