using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Questions.Models;
using Plato.Questions.Services;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Answers.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IEntityReplyStore<Answer> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IPostManager<Answer> _replyManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IEntityStore<Question> entityStore,
            IContextFacade contextFacade,
            IAuthorizationService authorizationService,
            IEntityReplyStore<Answer> entityReplyStore,
            IPostManager<Answer> replyManager,
            IAlerter alerter)
        {
      
            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _authorizationService = authorizationService;
            _entityReplyStore = entityReplyStore;
            _replyManager = replyManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> ToAnswer(string id)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Get permission
            var permission = entity.CreatedUserId == user.Id
                ? Permissions.MarkOwnRepliesAnswer
                : Permissions.MarkAnyReplyAnswer;

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, permission))
            {
                return Unauthorized();
            }
            
            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsAnswer = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);
            if (result.Succeeded)
            {
                await UpdateEntityAsync(entity);
                _alerter.Success(T["Marked As Answer Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not mark the reply as an answer"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Questions",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));

        }

        public async Task<IActionResult> FromAnswer(string id)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated
            if (user == null)
            {
                return Unauthorized();
            }
            
            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get reply
            var reply = await _entityReplyStore.GetByIdAsync(replyId);

            // Ensure reply exists
            if (reply == null)
            {
                return NotFound();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }
            
            // Get permission
            var permission = entity.CreatedUserId == user.Id
                ? Permissions.MarkOwnRepliesAnswer
                : Permissions.MarkAnyReplyAnswer;

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, permission))
            {
                return Unauthorized();
            }
            
            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsAnswer = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                await UpdateEntityAsync(entity);
                _alerter.Success(T["Answer Removed Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove the answer"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Questions",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply.Id
            }));
            
        }
        
        async Task<Question> UpdateEntityAsync(Question entity)
        {

            // Get a count of all replies marked as an answer
            var answers = await _entityReplyStore.QueryAsync()
                .Take(1)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.ShowAnswers.True();
                })
                .ToList();
            
            // Update answer details on entity 
            entity.TotalAnswers = answers?.Total ?? 0;
            
            // Update entity
            return await _entityStore.UpdateAsync(entity);

        }

    }

}
