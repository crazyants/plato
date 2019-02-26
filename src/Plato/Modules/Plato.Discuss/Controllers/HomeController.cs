using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Internal.Navigation;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<Topic> _topicViewProvider;
        private readonly IViewProviderManager<Reply> _replyViewProvider;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IPostManager<Topic> _topicManager;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAuthorizationService _authorizationService;
        private readonly IReplyService _replyService;

        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IEntityStore<Topic> entityStore,
            IViewProviderManager<Topic> topicViewProvider,
            IEntityReplyStore<Reply> entityReplyStore,
            IViewProviderManager<Reply> replyViewProvider,
            IPostManager<Topic> topicManager,
            IPostManager<Reply> replyManager,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IAuthorizationService authorizationService, 
            IReplyService replyService)
        {
            _topicViewProvider = topicViewProvider;
            _replyViewProvider = replyViewProvider;
            _entityStore = entityStore;
            _contextFacade = contextFacade;
            _entityReplyStore = entityReplyStore;
            _topicManager = topicManager;
            _replyManager = replyManager;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;
            _platoUserStore = platoUserStore;
            _authorizationService = authorizationService;
            _replyService = replyService;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Latest Topics
        // -----------------

        public async Task<IActionResult> Index(
            int offset,
            TopicIndexOptions opts,
            PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new TopicIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            if (offset > 0)
            {
                pager.Page = offset.ToSafeCeilingDivision(pager.PageSize);
                pager.SelectedOffset = offset;
            }
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"]);
            });

            await CreateSampleData();

            // Get default options
            var defaultViewOptions = new TopicIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            // Build infinite scroll options
            opts.Scroll = new ScrollOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };

            // Build view model
            var viewModel = new TopicIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adapters
            HttpContext.Items[typeof(TopicIndexViewModel)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTopics", viewModel);
            }

            // Return view
            return View(await _topicViewProvider.ProvideIndexAsync(new Topic(), this));

        }

        // -----------------
        // Popular Topics
        // -----------------

        public Task<IActionResult> Popular(
            TopicIndexOptions opts,
            PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new TopicIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            opts.Sort = SortBy.Replies;
            opts.Order = OrderBy.Desc;

            return Index(0, opts, pager);
        }

        // -----------------
        // New topic
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {

            if (!await _authorizationService.AuthorizeAsync(this.User, channel, Permissions.PostTopics))
            {
                return Unauthorized();
            }
            
            var topic = new Topic();
            if (channel > 0)
            {
                topic.CategoryId = channel;
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], discuss => discuss
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                ).Add(S["New Post"], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View(await _topicViewProvider.ProvideEditAsync(topic, this));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTopicViewModel model)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Validate model state within all view providers
            if (await _topicViewProvider.IsModelStateValid(new Topic()
            {
                Title = model.Title,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            }, this))
            {

                // Get composed type from all involved view providers
                var topic = await _topicViewProvider.GetComposedType(this);
                
                // Populated created by
                topic.CreatedUserId = user?.Id ?? 0;
                topic.CreatedDate = DateTimeOffset.UtcNow;

                // We need to first add the fully composed type
                // so we have a unique entity Id for all ProvideUpdateAsync
                // methods within any involved view provider
                var newTopic = await _topicManager.CreateAsync(topic);

                // Ensure the insert was successful
                if (newTopic.Succeeded)
                {

                    // Indicate new topic to prevent topic update
                    // on first creation within our topic view provider
                    newTopic.Response.IsNewTopic = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _topicViewProvider.ProvideUpdateAsync(newTopic.Response, this);

                    // Everything was OK
                    _alerter.Success(T["Topic Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Topic), new {Id = newTopic.Response.Id});

                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in newTopic.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create(0);

        }

        // -----------------
        // Display topic
        // -----------------

        public async Task<IActionResult> Topic(
            int id,
            int offset,
            TopicOptions opts,
            PagerOptions pager)
        {

            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission to view deleted topics
            if (topic.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId, Permissions.ViewDeletedTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Discuss",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // Ensure we have permission to view private topics
            if (topic.IsPrivate)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId, Permissions.ViewPrivateTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Discuss",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // Ensure we have permission to view spam topics
            if (topic.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId, Permissions.ViewSpamTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Discuss",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // default options
            if (opts == null)
            {
                opts = new TopicOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            if (offset > 0)
            {
                pager.Page = offset.ToSafeCeilingDivision(pager.PageSize);
                pager.SelectedOffset = offset;
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], discuss => discuss
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                ).Add(S[topic.Title.TrimToAround(75)], post => post
                    .LocalNav()
                );
            });

            // Maintain previous route data when generating page links
            // Get default options
            var defaultViewOptions = new TopicViewModel();
            var defaultPagerOptions = new PagerOptions();

            if (offset > 0 && !this.RouteData.Values.ContainsKey("offset"))
                this.RouteData.Values.Add("offset", offset);
            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.PageSize);
            
            opts.Params.EntityId = topic.Id;

            // Build infinite scroll options
            opts.Scroll = new ScrollOptions
            {
                Url = GetInfiniteScrollCallbackUrl()
            };

            // Build view model
            var viewModel = new TopicViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add models to context for use within view adaptors
            HttpContext.Items[typeof(TopicViewModel)] = viewModel;
            HttpContext.Items[typeof(Topic)] = topic;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                {
                    return View("GetTopicReplies", viewModel);
                }
            }
            
            // Return view
            return View(await _topicViewProvider.ProvideDisplayAsync(topic, this));

        }

        // -----------------
        // Post new reply
        // -----------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Topic))]
        public async Task<IActionResult> TopicPost(EditReplyViewModel model)
        {
            // We always need an entity to reply to
            var topic = await _entityStore.GetByIdAsync(model.EntityId);
            if (topic == null)
            {
                return NotFound();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build reply
            var reply = new Reply()
            {
                EntityId = model.EntityId,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // Validate model state within all view providers
            if (await _replyViewProvider.IsModelStateValid(reply, this))
            {

                // We need to first add the reply so we have a unique Id
                // for all ProvideUpdateAsync methods within any involved view providers
                var result = await _replyManager.CreateAsync(reply);

                // Ensure the insert was successful
                if (result.Succeeded)
                {

                    // Indicate this is a new reply so our view provider won't attempt to update
                    result.Response.IsNewReply = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _replyViewProvider.ProvideUpdateAsync(result.Response, this);

                    // Everything was OK
                    _alerter.Success(T["Reply Added Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Topic), new
                    {
                        Id = topic.Id,
                        Alias = topic.Alias
                    });

                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Topic(topic.Id, 0, null, null);

        }
      
        // -----------------
        // Edit topic
        // -----------------

        public async Task<IActionResult> Edit(int id)
        {
            // Get topic we are editing
            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user?.Id == topic.CreatedUserId
                    ? Permissions.EditOwnTopics
                    : Permissions.EditAnyTopic))
            {
                return Unauthorized();
            }
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S[topic.Title.TrimToAround(75)], post => post
                        .Action("Topic", "Home", "Plato.Discuss", new RouteValueDictionary()
                        {
                            ["Id"] = topic.Id,
                            ["Alias"] = topic.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Post"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View(await _topicViewProvider.ProvideEditAsync(topic, this));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditTopicViewModel model)
        {
            // Get entity we are editing 
            var topic = await _entityStore.GetByIdAsync(model.Id);
            if (topic == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _topicViewProvider.IsModelStateValid(new Topic()
            {
                Title = model.Title,
                Message = model.Message
            }, this))
            {

                // Get current user
                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // Only update edited information if the message changes
                if (model.Message != topic.Message)
                {
                    topic.EditedUserId = user?.Id ?? 0;
                    topic.EditedDate = DateTimeOffset.UtcNow;
                }

                // Always update modified information
                topic.ModifiedUserId = user?.Id ?? 0;
                topic.ModifiedDate = DateTimeOffset.UtcNow;
                
                // Update title & message
                topic.Title = model.Title;
                topic.Message = model.Message;

                // Execute view providers ProvideUpdateAsync method
                await _topicViewProvider.ProvideUpdateAsync(topic, this);

                // Everything was OK
                _alerter.Success(T["Topic Updated Successfully!"]);

                // Redirect to topic
                return RedirectToAction(nameof(Topic), new
                {
                    Id = topic.Id,
                    Alias = topic.Alias
                });

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create(0);

        }

        // -----------------
        // Edit reply
        // -----------------

        public async Task<IActionResult> EditReply(int id)
        {

            // Get reply we are editing
            var reply = await _entityReplyStore.GetByIdAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            // Get reply entity
            var topic = await _entityStore.GetByIdAsync(reply.EntityId);
            if (topic == null)
            {
                return NotFound();
            }
            
            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user?.Id == reply.CreatedUserId
                    ? Permissions.EditOwnReplies
                    : Permissions.EditAnyReply))
            {
                return Unauthorized();
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S[topic.Title.TrimToAround(75)], post => post
                        .Action("Topic", "Home", "Plato.Discuss", new RouteValueDictionary()
                        {
                            ["Id"] = topic.Id,
                            ["Alias"] = topic.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Reply"], post => post
                        .LocalNav()
                    );
            });
            
            var result = await _replyViewProvider.ProvideEditAsync(reply, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(EditReply))]
        public async Task<IActionResult> EditReplyPost(EditReplyViewModel model)
        {

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(model.Id);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var topic = await _entityStore.GetByIdAsync(reply.EntityId);
            if (topic == null)
            {
                return NotFound();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Only update edited information if the message changes
            if (model.Message != reply.Message)
            {
                reply.EditedUserId = user?.Id ?? 0;
                reply.EditedDate = DateTimeOffset.UtcNow;
            }

            // Always update modified date
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            
            // Update the message
            reply.Message = model.Message;
            
            // Validate model state within all view providers
            if (await _replyViewProvider.IsModelStateValid(reply, this))
            {

                // Execute view providers ProvideUpdateAsync method
                await _replyViewProvider.ProvideUpdateAsync(reply, this);

                // Everything was OK
                _alerter.Success(T["Reply Updated Successfully!"]);

                // Redirect to topic
                return RedirectToAction(nameof(Topic), new
                {
                    Id = topic.Id,
                    Alias = topic.Alias
                });

            }

            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create(0);

        }

        // -----------------
        // Report Topic
        // -----------------

        public Task<IActionResult> Report(
            int entityId,
            int entityReplyId = 0)
        {
            // Return view
            return Task.FromResult((IActionResult) View());

        }

        // -----------------
        // Delete / Restore Topic
        // -----------------

        public async Task<IActionResult> DeleteTopic(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get topic
            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user.Id == topic.CreatedUserId
                    ? Permissions.DeleteOwnTopics
                    : Permissions.DeleteAnyTopic))
            {
                return Unauthorized();
            }
            
            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsDeleted = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic deleted successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias
            }));
            
        }
     
        public async Task<IActionResult> RestoreTopic(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get topic
            var topic = await _entityStore.GetByIdAsync(entityId);

            // Ensure the topic exists
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user.Id == topic.CreatedUserId
                    ? Permissions.RestoreOwnTopics
                    : Permissions.RestoreAnyTopic))
            {
                return Unauthorized();
            }

            // Update topic
            topic.ModifiedUserId = user?.Id ?? 0;
            topic.ModifiedDate = DateTimeOffset.UtcNow;
            topic.IsDeleted = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(topic);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic restored successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the topic"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias
            }));

        }
        
        // -----------------
        // Delete / Restore Reply
        // -----------------
        
        public async Task<IActionResult> DeleteReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the topic exists
            var topic = await _entityStore.GetByIdAsync(reply.EntityId);
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.DeleteOwnReplies
                    : Permissions.DeleteAnyReply))
            {
                return Unauthorized();
            }

            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = true;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply deleted successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the reply"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias
            }));

        }

        public async Task<IActionResult> RestoreReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int replyId);
            if (!ok)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure we are authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the topic exists
            var topic = await _entityStore.GetByIdAsync(reply.EntityId);
            if (topic == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, topic.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.RestoreOwnReplies
                    : Permissions.RestoreAnyReply))
            {
                return Unauthorized();
            }

            // Update reply
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;
            reply.IsDeleted = false;

            // Save changes and return results
            var result = await _replyManager.UpdateAsync(reply);

            if (result.Succeeded)
            {
                _alerter.Success(T["Reply restored successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the reply"]);
            }

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias
            }));

        }

        // -----------------
        // Jump to reply
        // -----------------

        public async Task<IActionResult> Jump(
            int id,
            int replyId,
            TopicOptions opts,
            PagerOptions pager)
        {

            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            // default options
            if (opts == null)
            {
                opts = new TopicOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            // Set entity Id for replies to return
            opts.Params.EntityId = topic.Id;

            // We need to iterate all replies to calculate the offset
            pager.Page = 1;
            pager.PageSize = int.MaxValue;

            // Get offset for given reply
            var offset = 0;
            var replies = await _replyService.GetRepliesAsync(opts, pager);
            if (replies?.Data != null)
            {
                foreach (var reply in replies.Data)
                {
                    offset++;
                    if (reply.Id == replyId)
                    {
                        break;
                    }
                }
            }

            if (offset == 0)
            {
                // Could not locate offset, fallback by redirecting to topic
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["Area"] = "Plato.Discuss",
                    ["Controller"] = "Home",
                    ["Action"] = "Topic",
                    ["Id"] = topic.Id,
                    ["Alias"] = topic.Alias
                }));
            }

            // Redirect to offset within topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss",
                ["Controller"] = "Home",
                ["Action"] = "Topic",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias,
                ["Offset"] = offset
            }));

        }
        
        #endregion

        #region "Private Methods"
        
        string GetInfiniteScrollCallbackUrl()
        {

            RouteData.Values.Remove("pager.page");
            RouteData.Values.Remove("offset");
       
            return _contextFacade.GetRouteUrl(RouteData.Values);

        }
        
        string GetSampleMarkDown(int number)
        {
            return @"Hi There, 

This is just a sample post to demonstrate discussions within Plato. Discussions use markdown for formatting and can be organized using tags, labels or channels. 

You can add dozens of :large_blue_diamond: emojis :large_blue_diamond: and @mention other users within your posts. For example hey @admin.

We hope you enjoy this early version of Plato :)

Ryan :heartpulse: :heartpulse: :heartpulse:";

        }

        async Task CreateSampleData()
        {
            var users = await _platoUserStore.QueryAsync()
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();

            var rnd = new Random();
            var totalUsers = users?.Total - 1 ?? 0;
            var randomUser = users?.Data[rnd.Next(0, totalUsers)];

            var topic = new Topic()
            {
                Title = "Test Topic " + rnd.Next(0, 2000).ToString(),
                Message = GetSampleMarkDown(rnd.Next(0, 2000)),
                CreatedUserId = randomUser?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // create topic
            var data = await _topicManager.CreateAsync(topic);
            if (data.Succeeded)
            {
                for (var i = 0; i < 25; i++)
                {
                    rnd = new Random();
                    randomUser = users?.Data[rnd.Next(0, totalUsers)];

                    var reply = new Reply()
                    {
                        EntityId = data.Response.Id,
                        Message = GetSampleMarkDown(i) + " - reply: " + i.ToString(),
                        CreatedUserId = randomUser?.Id ?? 0,
                        CreatedDate = DateTimeOffset.UtcNow
                    };
                    var newReply = await _replyManager.CreateAsync(reply);
                }
            }
        }

        #endregion

    }

}
