using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Articles.Services;
using Plato.Articles.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
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

namespace Plato.Articles.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<Article> _entityViewProvider;
        private readonly IViewProviderManager<ArticleComment> _replyViewProvider;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<ArticleComment> _entityReplyStore;
        private readonly IPostManager<Article> _topicManager;
        private readonly IPostManager<ArticleComment> _replyManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<ArticleComment> _replyService;

        private readonly IPlatoUserStore<User> _platoUserStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IEntityStore<Article> entityStore,
            IViewProviderManager<Article> entityViewProvider,
            IEntityReplyStore<ArticleComment> entityReplyStore,
            IViewProviderManager<ArticleComment> replyViewProvider,
            IPostManager<Article> topicManager,
            IPostManager<ArticleComment> replyManager,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IAuthorizationService authorizationService,
            IEntityReplyService<ArticleComment> replyService)
        {
            _entityViewProvider = entityViewProvider;
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
        // Latest Entities
        // -----------------

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"]);
            });


            //await CreateSampleData();

            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
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
            
            // Build view model
            var viewModel = new EntityIndexViewModel<Article>()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adapters
            HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetArticles", viewModel);
            }

            // Return view
            return View(await _entityViewProvider.ProvideIndexAsync(new Article(), this));

        }

        // -----------------
        // Popular Entities
        // -----------------

        public Task<IActionResult> Popular(EntityIndexOptions opts, PagerOptions pager)
        {

            // default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            opts.Sort = SortBy.Replies;
            opts.Order = OrderBy.Desc;

            return Index(opts, pager);
        }

        // -----------------
        // New article
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {

            if (!await _authorizationService.AuthorizeAsync(this.User, channel, Permissions.CreateArticles))
            {
                return Unauthorized();
            }

            var topic = new Article();
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
                ).Add(S["Articles"], articles => articles
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                ).Add(S["New Article"], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View(await _entityViewProvider.ProvideEditAsync(topic, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTopicViewModel model)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Validate model state within all view providers
            if (await _entityViewProvider.IsModelStateValid(new Article()
            {
                Title = model.Title,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            }, this))
            {

                // Get composed type from all involved view providers
                var entity = await _entityViewProvider.GetComposedType(this);
                
                // Populated created by
                entity.CreatedUserId = user?.Id ?? 0;
                entity.CreatedDate = DateTimeOffset.UtcNow;

                // We need to first add the fully composed type
                // so we have a unique entity Id for all ProvideUpdateAsync
                // methods within any involved view provider
                var newTopic = await _topicManager.CreateAsync(entity);

                // Ensure the insert was successful
                if (newTopic.Succeeded)
                {

                    // Indicate new topic to prevent topic update
                    // on first creation within our topic view provider
                    newTopic.Response.IsNewTopic = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _entityViewProvider.ProvideUpdateAsync(newTopic.Response, this);

                    // Everything was OK
                    _alerter.Success(T["Article Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Display), new {Id = newTopic.Response.Id});

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
        // Display article
        // -----------------

        public async Task<IActionResult> Display(int id, int offset, EntityOptions opts, PagerOptions pager)
        {

            var entity = await _entityStore.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission to view deleted topics
            if (entity.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewDeletedTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Articles",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // Ensure we have permission to view private topics
            if (entity.IsPrivate)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewPrivateTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Articles",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // Ensure we have permission to view spam topics
            if (entity.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewSpamTopics))
                {
                    // Redirect back to main index
                    return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                    {
                        ["Area"] = "Plato.Articles",
                        ["Controller"] = "Home",
                        ["Action"] = "Index"
                    }));
                }
            }

            // default options
            if (opts == null)
            {
                opts = new EntityOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], articles => articles
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                ).Add(S[entity.Title.TrimToAround(75)], post => post
                    .LocalNav()
                );
            });

            // Maintain previous route data when generating page links
            // Get default options
            var defaultViewOptions = new EntityViewModel<Article, ArticleComment>();
            var defaultPagerOptions = new PagerOptions();

            if (offset > 0 && !this.RouteData.Values.ContainsKey("offset"))
                this.RouteData.Values.Add("offset", offset);
            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.PageSize);
            
            opts.EntityId = entity.Id;

            

            // Build view model
            var viewModel = new EntityViewModel<Article, ArticleComment>()
            {
                Options = opts,
                Pager = pager
            };

            // Add models to context
            HttpContext.Items[typeof(EntityViewModel<Article, ArticleComment>)] = viewModel;
            HttpContext.Items[typeof(Article)] = entity;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                {
                    return View("GetArticleComments", viewModel);
                }
            }
            
            // Return view
            return View(await _entityViewProvider.ProvideDisplayAsync(entity, this));

        }

        // -----------------
        // Post comment
        // -----------------

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Display))]
        public async Task<IActionResult> DisplayPost(EditReplyViewModel model)
        {
            // We always need an entity to reply to
            var entity = await _entityStore.GetByIdAsync(model.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build reply
            var reply = new ArticleComment()
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
                    return RedirectToAction(nameof(Display), new
                    {
                        Id = entity.Id,
                        Alias = entity.Alias
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

            return await Display(entity.Id, 0, null, null);

        }
      
        // -----------------
        // Edit article
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
                    ? Permissions.EditArticles
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
                    ).Add(S["Articles"], articles => articles
                        .Action("Index", "Home", "Plato.Articles")
                        .LocalNav()
                    ).Add(S[topic.Title.TrimToAround(75)], post => post
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary()
                        {
                            ["Id"] = topic.Id,
                            ["Alias"] = topic.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Article"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View(await _entityViewProvider.ProvideEditAsync(topic, this));

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
            if (await _entityViewProvider.IsModelStateValid(new Article()
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
                await _entityViewProvider.ProvideUpdateAsync(topic, this);

                // Everything was OK
                _alerter.Success(T["Topic Updated Successfully!"]);

                // Redirect to topic
                return RedirectToAction(nameof(Display), new
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
                    ? Permissions.EditOwnComment
                    : Permissions.EditAnyComment))
            {
                return Unauthorized();
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Articles"], articles => articles
                        .Action("Index", "Home", "Plato.Articles")
                        .LocalNav()
                    ).Add(S[topic.Title.TrimToAround(75)], post => post
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary()
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
                return RedirectToAction(nameof(Display), new
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
                    ? Permissions.DeleteArticles
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

            // Redirect back to article
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Articles",
                ["Controller"] = "Home",
                ["Action"] = "Display",
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

            // Redirect back to article
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Articles",
                ["Controller"] = "Home",
                ["Action"] = "Display",
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

            // Redirect back to article
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Articles",
                ["Controller"] = "Home",
                ["Action"] = "Display",
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

            // Redirect back to article
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Articles",
                ["Controller"] = "Home",
                ["Action"] = "Display",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias
            }));

        }

        // -----------------
        // Jump
        // -----------------

        public async Task<IActionResult> Jump(
            int id,
            int replyId,
            EntityOptions opts,
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
                opts = new EntityOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            // Set entity Id for replies to return
            opts.EntityId = topic.Id;

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
                    ["Area"] = "Plato.Articles",
                    ["Controller"] = "Home",
                    ["Action"] = "Display",
                    ["Id"] = topic.Id,
                    ["Alias"] = topic.Alias
                }));
            }

            // Redirect to offset within article
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Articles",
                ["Controller"] = "Home",
                ["Action"] = "Display",
                ["Id"] = topic.Id,
                ["Alias"] = topic.Alias,
                ["Offset"] = offset
            }));

        }
        
        #endregion

        #region "Private Methods"
        
        
        string GetSampleMarkDown(int number)
        {
            return @"Hi There, 

This is just a sample article to demonstrate articles within Plato. Articles use markdown for formatting and can be organized using tags, labels or categories. 

We hope you enjoy this early version of Plato :)

        string GetSampleMarkDown(int number)
Ryan :heartpulse: :heartpulse: :heartpulse:" + number;

        }

        async Task CreateSampleData()
        {
            
            var users = await _platoUserStore.QueryAsync()
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();

            var rnd = new Random();
            var totalUsers = users?.Total - 1 ?? 0;
            var randomUser = users?.Data[rnd.Next(0, totalUsers)];

            var topic = new Article()
            {
                Title = "Test Article " + rnd.Next(0, 2000).ToString(),
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

                    var reply = new ArticleComment()
                    {
                        EntityId = data.Response.Id,
                        Message = GetSampleMarkDown(i) + " - comment : " + i.ToString(),
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
