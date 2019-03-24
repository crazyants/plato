using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Questions.Models;
using Plato.Questions.Services;


namespace Plato.Questions.Answers.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IPostManager<Question> _topicManager;
        private readonly IPostManager<Answer> _replyManager;
        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IEntityReplyStore<Answer> _entityReplyStore;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IAlerter _alerter;
        private readonly IAuthorizationService _authorizationService;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IPlatoUserStore<User> userStore, 
            IEntityStore<Question> entityStore,
            IContextFacade contextFacade,
            IAuthorizationService authorizationService,
            IPostManager<Question> topicManager,
            IAlerter alerter,
            IEntityReplyStore<Answer> entityReplyStore,
            IPostManager<Answer> replyManager)
        {
            _userStore = userStore;
            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _authorizationService = authorizationService;
            _topicManager = topicManager;
            _alerter = alerter;
            _entityReplyStore = entityReplyStore;
            _replyManager = replyManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        #region "Replies"

        public async Task<IActionResult> ToAnswer(string id)
        {

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

            // Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, ModeratorPermissions.HideReplies))
            //{
            //    return Unauthorized();
            //}

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsAnswer = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Marked As Answer Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not mark the reply as an asnwer"]);
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

            //// Ensure we have permission
            //if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, ModeratorPermissions.ShowReplies))
            //{
            //    return Unauthorized();
            //}

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsAnswer = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
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
        
        #endregion

    }

}
