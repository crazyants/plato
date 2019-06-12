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
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Client.Services;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Stores;

namespace Plato.Discuss.StopForumSpam.Controllers
{
    public class HomeController : Controller
    {

        private readonly ISpamSettingsStore<SpamSettings> _spamSettingsStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ISpamClient _spamClient;
        private readonly IAlerter _alerter;
   
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer<HomeController> htmlLocalizer,
            IStringLocalizer<HomeController> stringLocalizer,
            ISpamSettingsStore<SpamSettings> spamSettingsStore,
            IAuthorizationService authorizationService,
            IEntityReplyStore<Reply> entityReplyStore,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Topic> entityStore,
            IContextFacade contextFacade,
            ISpamClient spamClient,
            IAlerter alerter)
        {
            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _authorizationService = authorizationService;
            _spamSettingsStore = spamSettingsStore;
            _entityReplyStore = entityReplyStore;
            _platoUserStore = platoUserStore;
            _spamClient = spamClient;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        #region "Topics"
        
        public async Task<IActionResult> AddSpammer(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.AddToStopForumSpam))
            {
                return Unauthorized();
            }

            // Get user for topic
            var user = await _platoUserStore.GetByIdAsync(entity.CreatedUserId);
           
            // Ensure the user exists
            if (user == null)
            {
                return NotFound();
            }

            // Configure spam client
            await ConfigureSpamClient();
            
            // Add the user
            var result = await _spamClient.AddSpammerAsync(
                user.UserName,
                user.Email,
                user.IpV4Address);
            
            if (result.Success)
            {
                _alerter.Success(T["Spammer Added Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not add the user details to the StopForumSpam database"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }
              
        #endregion

        #region "Replies"

        ////public async Task<IActionResult> HideReply(string id)
        ////{

        ////    // Ensure we have a valid id
        ////    var ok = int.TryParse(id, out var replyId);
        ////    if (!ok)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var reply = await _entityReplyStore.GetByIdAsync(replyId);
        ////    if (reply == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var topic = await _entityStore.GetByIdAsync(reply.EntityId);

        ////    // Ensure the topic exists
        ////    if (topic == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    // Ensure we have permission
        ////    if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.HideReplies))
        ////    {
        ////        return Unauthorized();
        ////    }

        ////    var user = await _contextFacade.GetAuthenticatedUserAsync();

        ////    // Update topic
        ////    reply.ModifiedUserId = user?.Id ?? 0;
        ////    reply.ModifiedDate = DateTimeOffset.UtcNow;
        ////    reply.IsHidden = true;

        ////    // Save changes and return results
        ////    var result = await _replyManager.UpdateAsync(reply);

        ////    if (result.Succeeded)
        ////    {
        ////        _alerter.Success(T["Reply Hidden Successfully"]);
        ////    }
        ////    else
        ////    {
        ////        _alerter.Danger(T["Could not hide the reply"]);
        ////    }

        ////    // Redirect back to reply
        ////    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
        ////    {
        ////        ["area"] = "Plato.Discuss",
        ////        ["controller"] = "Home",
        ////        ["action"] = "Reply",
        ////        ["opts.id"] = topic.Id,
        ////        ["opts.alias"] = topic.Alias,
        ////        ["opts.replyId"] = reply.Id
        ////    }));

        ////}

        ////public async Task<IActionResult> ShowReply(string id)
        ////{

        ////    // Ensure we have a valid id
        ////    var ok = int.TryParse(id, out var replyId);
        ////    if (!ok)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var reply = await _entityReplyStore.GetByIdAsync(replyId);

        ////    if (reply == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var topic = await _entityStore.GetByIdAsync(reply.EntityId);

        ////    // Ensure the topic exists
        ////    if (topic == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    // Ensure we have permission
        ////    if (!await _authorizationService.AuthorizeAsync(User, topic.CategoryId, ModeratorPermissions.ShowReplies))
        ////    {
        ////        return Unauthorized();
        ////    }

        ////    var user = await _contextFacade.GetAuthenticatedUserAsync();

        ////    // Update topic
        ////    reply.ModifiedUserId = user?.Id ?? 0;
        ////    reply.ModifiedDate = DateTimeOffset.UtcNow;
        ////    reply.IsHidden = false;

        ////    // Save changes and return results
        ////    var result = await _replyManager.UpdateAsync(reply);

        ////    if (result.Succeeded)
        ////    {
        ////        _alerter.Success(T["Reply Made Public Successfully"]);
        ////    }
        ////    else
        ////    {
        ////        _alerter.Danger(T["Could not make the reply public"]);
        ////    }
        ////    // Redirect back to reply
        ////    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
        ////    {
        ////        ["area"] = "Plato.Discuss",
        ////        ["controller"] = "Home",
        ////        ["action"] = "Reply",
        ////        ["opts.id"] = topic.Id,
        ////        ["opts.alias"] = topic.Alias,
        ////        ["opts.replyId"] = reply.Id
        ////    }));


        ////}

        #endregion

        async Task ConfigureSpamClient()
        {
            // Get spam settings
            var settings = await _spamSettingsStore.GetAsync();

            // Ensure we have settings
            if (settings == null)
            {
                throw new Exception("No spam settings have been configured!");
            }

            // Ensure we have an api key
            if (String.IsNullOrEmpty(settings.ApiKey))
            {
                throw new Exception("A StopForumSpam API key is required!");
            }

            // Configure spam client
            _spamClient.Configure(o => { o.ApiKey = settings.ApiKey; });

        }

    }

}
