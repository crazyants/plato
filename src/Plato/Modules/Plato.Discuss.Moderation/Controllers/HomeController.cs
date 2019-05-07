using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Moderation.Controllers
{
    public class HomeController : Controller
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IPostManager<Topic> _topicManager;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;
   
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
     
            IEntityStore<Topic> entityStore,
            IContextFacade contextFacade,
            IAuthorizationService authorizationService,
            IPostManager<Topic> topicManager,
            IAlerter alerter,
            IEntityReplyStore<Reply> entityReplyStore,
            IPostManager<Reply> replyManager)
        {
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

        #region "Topics"
        
        public async Task<IActionResult> Pin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.PinTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsPinned = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Pinned Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove topic from SPAM"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> Unpin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.UnpinTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsPinned = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Pin Removed Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove pin"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }
        
        public async Task<IActionResult> Hide(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.HideTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsHidden = true;
            
            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);
            
            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Hidden Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not hide the topic"]);
            }
            
            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> Show(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.ShowTopics))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsHidden = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Made Public Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not update the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }
        
        public async Task<IActionResult> Lock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.LockTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsLocked = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Closed Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not close the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> Unlock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.UnlockTopics))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsLocked = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Opened Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not open the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> ToSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.TopicToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsSpam = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Marked as SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not mark topic as SPAM"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> FromSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.TopicFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsSpam = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Removed from SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove topic from SPAM"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.DeleteTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsDeleted = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> Restore(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.RestoreTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsDeleted = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }
   
        #endregion

        #region "Replies"

        public async Task<IActionResult> HideReply(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.HideReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsHidden = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Hidden Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not hide the reply"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));

        }

        public async Task<IActionResult> ShowReply(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.ShowReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsHidden = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Made Public Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not make the reply public"]);
            }
            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> ReplyToSpam(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.ReplyToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsSpam = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Marked as SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not mark the reply as SPAM"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> ReplyFromSpam(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.ReplyFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsSpam = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Removed from SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove the reply from SPAM"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> DeleteReply(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.DeleteReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the reply"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }

        public async Task<IActionResult> RestoreReply(string id)
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

            var topic = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.RestoreReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the reply"]);
            }

            // Redirect back to reply
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias,
                ["opts.replyId"] = reply.Id
            }));


        }
        
        #endregion

    }

}
