using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Discuss.Models;
using Plato.Discuss.StopForumSpam.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.Client.Services;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;

namespace Plato.Discuss.StopForumSpam.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ISpamSettingsStore<SpamSettings> _spamSettingsStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ISpamChecker _spamChecker;
        private readonly ISpamClient _spamClient;
        private readonly IAlerter _alerter;

        private readonly PlatoOptions _platoOpts;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer<HomeController> htmlLocalizer,
            IStringLocalizer<HomeController> stringLocalizer,
            ISpamSettingsStore<SpamSettings> spamSettingsStore,
            IAuthorizationService authorizationService,
            IEntityReplyStore<Reply> entityReplyStore,
            IPlatoUserStore<User> platoUserStore,
            IOptions<PlatoOptions> platoOpts,
            IEntityStore<Topic> entityStore,
            IContextFacade contextFacade,
            ISpamChecker spamChecker, 
            ISpamClient spamClient,
            IAlerter alerter)
        {
  
            _authorizationService = authorizationService;
            _spamSettingsStore = spamSettingsStore;
            _entityReplyStore = entityReplyStore;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _platoOpts = platoOpts.Value;
            _spamChecker = spamChecker;
            _entityStore = entityStore;
            _spamClient = spamClient;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        // -----------------
        // Index
        // Displays a summary from StopForumSpam.
        // -----------------

        public async Task<IActionResult> Index(EntityOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityOptions();
            }

            // We always need an entity Id
            if (opts.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(opts.Id));
            }

            // We always need an entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ViewStopForumSpam))
            {
                return Unauthorized();
            }
            
            // Get reply
            IEntityReply reply = null;
            if (opts.ReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(opts.ReplyId);
                if (reply == null)
                {
                    return NotFound();
                }
            }
            
            // Get user to validate
            var user = reply != null
                ? await GetUserToValidateAsync(reply)
                : await GetUserToValidateAsync(entity);

            // Ensure we found the user
            if (user == null)
            {
                return NotFound();
            }

            // Build view model
            var viewModel = new StopForumSpamViewModel()
            {
                Options = opts,
                Checker = await _spamChecker.CheckAsync(user)
            };

            // Return view
            return View(viewModel);

        }

        // -----------------
        // AddSpammer
        // -----------------

        public async Task<IActionResult> AddSpammer(EntityOptions opts)
        {

            // Disable functionality within demo mode
            if (_platoOpts.DemoMode)
            {
                return Unauthorized();
            }

            // Empty options
            if (opts == null)
            {
                opts = new EntityOptions();
            }

            // Validate
            if (opts.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(opts.Id));
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure the topic exists
            if (entity == null)
            {
                return NotFound();
            }
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, 
                entity.CategoryId, Permissions.AddToStopForumSpam))
            {
                return Unauthorized();
            }
            
            // Get reply
            IEntityReply reply = null;
            if (opts.ReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(opts.ReplyId);
            }
            
            // Get user for reply or entity
            var user = reply != null 
                ? await GetUserToValidateAsync(reply)
                : await GetUserToValidateAsync(entity);

            // Ensure we found the user
            if (user == null)
            {
                return NotFound();
            }

            // Add spammer for reply or entity
            var result = await AddSpammerAsync(user);

            // Confirmation
            if (result.Success)
            {
                _alerter.Success(T["Spammer Added Successfully"]);
            }
            else
            {
                _alerter.Danger(!string.IsNullOrEmpty(result.Error)
                    ? T[result.Error]
                    : T["An unknown error occurred adding the user to the StopForumSpam database."]);
            }

            // Redirect back to reply
            if (reply != null)
            {
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Reply",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
                    ["opts.replyId"] = reply.Id
                }));
            }

            // Redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }
        
        // ----------------------

        async Task<Response> AddSpammerAsync(IUser user)
        {

            // Configure client
            ConfigureSpamClient();

            // Add the user & return the result
            return await _spamClient.AddSpammerAsync(
                user.UserName,
                user.Email,
                user.IpV4Address);

        }
        
        async Task<User> GetUserToValidateAsync(IEntity entity)
        {

            // Get author
            var user = await _platoUserStore.GetByIdAsync(entity.CreatedUserId);

            // Ensure we found the user
            if (user == null)
            {
                return null;
            }

            // Use IP information from entity

            if (!string.IsNullOrEmpty(entity.IpV4Address))
            {
                if (!entity.IpV4Address.Equals(user.IpV4Address, StringComparison.OrdinalIgnoreCase))
                {
                    user.IpV4Address = entity.IpV4Address;
                }
            }

            if (!string.IsNullOrEmpty(entity.IpV6Address))
            {
                if (!entity.IpV6Address.Equals(user.IpV6Address, StringComparison.OrdinalIgnoreCase))
                {
                    user.IpV6Address = entity.IpV6Address;
                }
            }
            
            return user;

        }

        async Task<User> GetUserToValidateAsync(IEntityReply reply)
        {

            // Get author
            var user = await _platoUserStore.GetByIdAsync(reply.CreatedUserId);

            // Ensure we found the user
            if (user == null)
            {
                return null;
            }

            // Use IP information from reply

            if (!string.IsNullOrEmpty(reply.IpV4Address))
            {
                if (!reply.IpV4Address.Equals(user.IpV4Address, StringComparison.OrdinalIgnoreCase))
                {
                    user.IpV4Address = reply.IpV4Address;
                }
            }

            if (!string.IsNullOrEmpty(reply.IpV6Address))
            {
                if (!reply.IpV6Address.Equals(user.IpV6Address, StringComparison.OrdinalIgnoreCase))
                {
                    user.IpV6Address = reply.IpV6Address;
                }
            }

            return user;

        }

        void ConfigureSpamClient()
        {
            // Configure spam client
            _spamClient.Configure(async o => { o.ApiKey = await GetStopForumSpamApiKeyAsync(); });
        }
        
        async Task<string> GetStopForumSpamApiKeyAsync()
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

            return settings.ApiKey;

        }

    }

}
