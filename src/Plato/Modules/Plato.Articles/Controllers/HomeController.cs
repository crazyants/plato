using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Articles.Services;
using Plato.Entities;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Articles.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<UserIndex> _userIndexProvider;
        private readonly IViewProviderManager<Article> _entityViewProvider;
        private readonly IViewProviderManager<Comment> _replyViewProvider;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<Comment> _entityReplyStore;
        private readonly IPostManager<Article> _topicManager;
        private readonly IPostManager<Comment> _replyManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Comment> _replyService;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IFeatureFacade _featureFacade;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer<HomeController> stringLocalizer,
            IHtmlLocalizer<HomeController> localizer,
            IContextFacade contextFacade,
            IEntityStore<Article> entityStore,
            IViewProviderManager<Article> entityViewProvider,
            IEntityReplyStore<Comment> entityReplyStore,
            IViewProviderManager<Comment> replyViewProvider,
            IPostManager<Article> topicManager,
            IPostManager<Comment> replyManager,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            IPlatoUserStore<User> platoUserStore,
            IAuthorizationService authorizationService,
            IEntityReplyService<Comment> replyService,
            IViewProviderManager<UserIndex> userIndexProvider,
            IFeatureFacade featureFacade)
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
            _userIndexProvider = userIndexProvider;
            _featureFacade = featureFacade;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Latest
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
            
            await CreateSampleData();

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
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Article>)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetArticles", viewModel);
            }
            
            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Index"
            });

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"]);
            });
            
            // Return view
            return View(await _entityViewProvider.ProvideIndexAsync(new Article(), this));

        }

        // -----------------
        // Popular
        // -----------------

        public Task<IActionResult> Popular(EntityIndexOptions opts, PagerOptions pager)
        {

            // Default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            opts.Sort = SortBy.Replies;
            opts.Order = OrderBy.Desc;

            return Index(opts, pager);
        }

        // -----------------
        // New Entity
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {

            if (!await _authorizationService.AuthorizeAsync(this.User, channel, Permissions.PostArticles))
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
        public async Task<IActionResult> CreatePost(EditEntityViewModel model)
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
        // Display Entity
        // -----------------

        public async Task<IActionResult> Display(EntityOptions opts, PagerOptions pager)
        {

            // Get entity to display
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission to view deleted entities
            if (entity.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewDeletedArticles))
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

            // Ensure we have permission to view private entities
            if (entity.IsPrivate)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewPrivateArticles))
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

            // Ensure we have permission to view spam entities
            if (entity.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId, Permissions.ViewSpamArticles))
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

            // Default options
            if (opts == null)
            {
                opts = new EntityOptions();
            }

            // Default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Maintain previous route data when generating page links
            var defaultViewOptions = new EntityViewModel<Article, Comment>();
            var defaultPagerOptions = new PagerOptions();
            
            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.PageSize);
            
            // Build view model
            var viewModel = GetDisplayViewModel(entity, opts, pager);

            // Add models to context
            HttpContext.Items[typeof(EntityViewModel<Article, Comment>)] = viewModel;
            HttpContext.Items[typeof(Article)] = entity;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                {
                    return View("GetArticleComments", viewModel);
                }
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
                ).Add(S[entity.Title.TrimToAround(75)], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View(await _entityViewProvider.ProvideDisplayAsync(entity, this));

        }

        // -----------------
        // Post Reply
        // -----------------

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Display))]
        public async Task<IActionResult> DisplayPost(EditEntityReplyViewModel model)
        {

            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build reply
            var reply = new Comment()
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
                    _alerter.Success(T["Comment Added Successfully!"]);

                    // Redirect
                    return RedirectToAction(nameof(Reply), new RouteValueDictionary()
                    {
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias,
                        ["opts.replyId"] = result.Response.Id
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

            return await Display(new EntityOptions()
            {
                Id = entity.Id
            }, null);

        }
      
        // -----------------
        // Edit Entity
        // -----------------

        public async Task<IActionResult> Edit(EntityOptions opts)
        {
            // Get topic we are editing
            var entity = await _entityStore.GetByIdAsync(opts.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to edit
            if (user == null)
            {
                return Unauthorized();
            }

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user?.Id == entity.CreatedUserId
                    ? Permissions.EditOwnArticles
                    : Permissions.EditAnyArticle))
            {
                return Unauthorized();
            }
            
            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Articles"], index => index
                        .Action("Index", "Home", "Plato.Articles")
                        .LocalNav()
                    ).Add(S[entity.Title.TrimToAround(75)], display => display
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Article"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View(await _entityViewProvider.ProvideEditAsync(entity, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditEntityViewModel model)
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
        // Edit Reply
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
                            ["opts.id"] = topic.Id,
                            ["opts.alias"] = topic.Alias
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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditReply))]
        public async Task<IActionResult> EditReplyPost(EditEntityReplyViewModel model)
        {

            // Ensure the reply exists
            var reply = await _entityReplyStore.GetByIdAsync(model.Id);
            if (reply == null)
            {
                return NotFound();
            }

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
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

                // Redirect
                return RedirectToAction(nameof(Reply), new RouteValueDictionary()
                {
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias,
                    ["opts.replyId"] = reply.Id
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
        // Report Entity
        // -----------------

        public Task<IActionResult> Report(EntityOptions opts)
        {

            if (opts == null)
            {
                opts = new EntityOptions();
            }

            var viewModel = new ReportEntityViewModel()
            {
                Options = opts,
                AvailableReportReasons = GetReportReasons()
            };

            // Return view
            return Task.FromResult((IActionResult)View(viewModel));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Report))]
        public async Task<IActionResult> ReportPost(ReportEntityViewModel model)
        {

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(model.Options.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure the reply exists
            Comment reply = null;
            if (model.Options.ReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(model.Options.ReplyId);
                if (reply == null)
                {
                    return NotFound();
                }
            }

            _alerter.Success(reply != null
                ? T["Comment Reported Successfully!"]
                : T["Article Reported Successfully!"]);
            
            // Redirect
            return RedirectToAction(nameof(Reply), new RouteValueDictionary()
            {
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            });

        }
        
        // -----------------
        // Delete / Restore Entity
        // -----------------

        public async Task<IActionResult> Delete(string id)
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
                    ? Permissions.DeleteOwnArticles
                    : Permissions.DeleteAnyArticle))
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
                ["area"] = "Plato.Articles",
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
                    ? Permissions.RestoreOwnArticles
                    : Permissions.RestoreAnyArticle))
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
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
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
                    ? Permissions.DeleteOwnComments
                    : Permissions.DeleteAnyComment))
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
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
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
                    ? Permissions.RestoreOwnComments
                    : Permissions.RestoreAnyComment))
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
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = topic.Id,
                ["opts.alias"] = topic.Alias
            }));

        }

        // -----------------
        // Reply
        // -----------------

        public async Task<IActionResult> Reply(EntityOptions opts)
        {
            
            // Default options
            if (opts == null)
            {
                opts = new EntityOptions();
            }
            
            // Get entity
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }
            
            // Get offset for given reply
            var offset = 0;
            if (opts.ReplyId > 0)
            {
                // We need to iterate all replies to calculate the offset
                var replies = await _replyService.GetRepliesAsync(opts, new PagerOptions
                {
                    PageSize = int.MaxValue
                });
                if (replies?.Data != null)
                {
                    foreach (var reply in replies.Data)
                    {
                        offset++;
                        if (reply.Id == opts.ReplyId)
                        {
                            break;
                        }
                    }
                }
            }

            if (offset == 0)
            {
                // Could not locate offset, fallback by redirecting to entity
                return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["area"] = "Plato.Articles",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias
                }));
            }

            // Redirect to offset within entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Articles",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["pager.offset"] = offset,
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityIndexViewModel<Article>> GetIndexViewModelAsync(EntityIndexOptions options, PagerOptions pager)
        {

            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            // Restrict results to current feature
            if (feature != null)
            {
                options.FeatureId = feature.Id;
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Return updated model
            return new EntityIndexViewModel<Article>()
            {
                Options = options,
                Pager = pager
            };

        }

        EntityViewModel<Article, Comment> GetDisplayViewModel(Article entity, EntityOptions options, PagerOptions pager)
        {

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Ensure view model is aware of the entity we are displaying
            options.Id = entity.Id;

            // Return updated view model
            return new EntityViewModel<Article, Comment>()
            {
                Entity = entity,
                Options = options,
                Pager = pager
            };
        }

        IEnumerable<SelectListItem> GetReportReasons()
        {

            var output = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            foreach (var reason in ReportEntity.Reasons)
            {
                output.Add(new SelectListItem
                {
                    Text = S[reason.Value],
                    Value = Convert.ToString((int)reason.Key)
                });
            }

            return output;
        }
        
        // --------------

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
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            var topic = new Article()
            {
                Title = "Test Article " + rnd.Next(0, 2000).ToString(),
                Message = GetSampleMarkDown(rnd.Next(0, 2000)),
                FeatureId = feature?.Id ?? 0,
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

                    var reply = new Comment()
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
