using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
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
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Net.Abstractions;
using Plato.Entities.Extensions;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IReportEntityManager<Topic> _reportEntityManager;
        private readonly IReportEntityManager<Reply> _reportReplyManager;
        private readonly IViewProviderManager<Topic> _entityViewProvider;
        private readonly IViewProviderManager<Reply> _replyViewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IEntityReplyService<Reply> _replyService;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IClientIpAddress _clientIpAddress;
        private readonly IPostManager<Topic> _topicManager;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IReportEntityManager<Topic> reportEntityManager,
            IReportEntityManager<Reply> reportReplyManager,
            IViewProviderManager<Topic> entityViewProvider,
            IViewProviderManager<Reply> replyViewProvider,
            IAuthorizationService authorizationService,
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityReplyService<Reply> replyService,
            IPlatoUserStore<User> platoUserStore,
            IBreadCrumbManager breadCrumbManager,
            IPageTitleBuilder pageTitleBuilder,
            IClientIpAddress clientIpAddress,
            IPostManager<Topic> topicManager,
            IPostManager<Reply> replyManager,
            IEntityStore<Topic> entityStore,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            IAlerter alerter)
        {
            _authorizationService = authorizationService;
            _reportEntityManager = reportEntityManager;
            _reportReplyManager = reportReplyManager;
            _entityViewProvider = entityViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _replyViewProvider = replyViewProvider;
            _entityReplyStore = entityReplyStore;
            _pageTitleBuilder = pageTitleBuilder;
            _clientIpAddress = clientIpAddress;
            _platoUserStore = platoUserStore;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _replyManager = replyManager;
            _topicManager = topicManager;
            _replyService = replyService;
            _entityStore = entityStore;
            _alerter = alerter;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Index 
        // -----------------

        public async Task<IActionResult> Index(EntityIndexOptions opts, PagerOptions pager)
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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = await GetIndexViewModelAsync(opts, pager);

            // Add view model to context
            HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] = viewModel;

            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTopics", viewModel);
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
                ).Add(S["Discuss"]);
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideIndexAsync(new Topic(), this));

        }
        
        // -----------------
        // New Entity
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {

            if (!await _authorizationService.AuthorizeAsync(this.User, channel, Permissions.PostTopics))
            {
                return Unauthorized();
            }

            var entity = new Topic();
            if (channel > 0)
            {
                entity.CategoryId = channel;
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav())
                    .Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav())
                    .Add(S["New Post"], post => post
                        .LocalNav());
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideEditAsync(entity, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditEntityViewModel model)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Build topic
            var entity = new Topic()
            {
                Title = model.Title,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow,
                IpV4Address = _clientIpAddress.GetIpV4Address(), // "77.218.241.112"
                IpV6Address = _clientIpAddress.GetIpV6Address()
            };

            // Validate model state within all view providers
            if (await _entityViewProvider.IsModelStateValidAsync(entity, this))
            {

                // Get composed model from all involved view providers
                entity = await _entityViewProvider.ComposeModelAsync(entity, this);

                // Create entity
                var result = await _topicManager.CreateAsync(entity);

                // Ensure the insert was successful
                if (result.Succeeded)
                {

                    // Indicate new entity to prevent entity update
                    // on first creation within our view provider
                    result.Response.IsNew = true;

                    // Execute view providers ProvideUpdateAsync method
                    await _entityViewProvider.ProvideUpdateAsync(result.Response, this);
                    
                    // Get authorize result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {

                        // Everything was OK
                        _alerter.Success(T["Topic Added Successfully!"]);

                        // Redirect to entity
                        return RedirectToAction(nameof(Display), new RouteValueDictionary()
                        {
                            ["opts.id"] = result.Response.Id,
                            ["opts.alias"] = result.Response.Alias
                        });

                    }

                    // Add any authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to index
                    return RedirectToAction(nameof(Index));
                    
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

            return await Create(0);

        }

        // -----------------
        // Display Entity
        // -----------------

        public async Task<IActionResult> Display(EntityOptions opts, PagerOptions pager)
        {

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
            
            // We always need an entity Id to display
            if (opts.Id <= 0)
            {
                return NotFound();
            }

            // Get entity to display
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission to view the entity
            var authorizeResult = await AuthorizeAsync(entity);
            if (!authorizeResult.Succeeded)
            {
                // Return 401
                return Unauthorized();
            }
            
            // Ensure we have permission to view private entities
            if (entity.IsPrivate)
            {

                // Get authenticated user
                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // IF we didn't create this entity ensure we have permission to view private entities
                if (entity.CreatedBy.Id != user?.Id)
                {
                    // Do we have permission to view private entities?
                    if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                        Permissions.ViewPrivateTopics))
                    {
                        // Return 401
                        return Unauthorized();
                    }
                }

            }

            // Maintain previous route data when generating page links
            var defaultViewOptions = new EntityViewModel<Topic, Reply>();
            var defaultPagerOptions = new PagerOptions();

            if (pager.Page != defaultPagerOptions.Page && !this.RouteData.Values.ContainsKey("pager.page"))
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.Size != defaultPagerOptions.Size && !this.RouteData.Values.ContainsKey("pager.size"))
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = GetDisplayViewModel(entity, opts, pager);

            // Add models to context 
            HttpContext.Items[typeof(EntityViewModel<Topic, Reply>)] = viewModel;
            HttpContext.Items[typeof(Topic)] = entity;
            
            // If we have a pager.page querystring value return paged view
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetTopicReplies", viewModel);
            }
            
            // Return Url for authentication purposes
            ViewData["ReturnUrl"] = _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            });

            // Build page title
            _pageTitleBuilder.AddSegment(S[entity.Title], int.MaxValue);

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], discuss => discuss
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                ).Add(S[entity.Title.TrimToAround(75)], post => post
                    .LocalNav()
                );
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideDisplayAsync(entity, this));

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
            var reply = new Reply()
            {
                EntityId = model.EntityId,
                Message = model.Message,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow,
                IpV4Address = _clientIpAddress.GetIpV4Address(), // "77.218.241.112",
                IpV6Address = _clientIpAddress.GetIpV6Address()
            };

            // Validate model state within all involved view providers
            if (await _replyViewProvider.IsModelStateValidAsync(reply, this))
            {

                // Get composed type from all involved view providers
                reply = await _replyViewProvider.ComposeModelAsync(reply, this);
                
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
                    
                    // Get authorization result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {

                        // Everything was OK
                        _alerter.Success(T["Reply Added Successfully!"]);

                        // Redirect to reply
                        return RedirectToAction(nameof(Reply), new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias,
                            ["opts.replyId"] = result.Response.Id
                        });
                        
                    }

                    // Add authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to entity
                    return RedirectToAction(nameof(Display), new RouteValueDictionary()
                    {
                        ["opts.id"] = entity.Id,
                        ["opts.alias"] = entity.Alias
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

            // Get entity 
            var entity = await _entityStore.GetByIdAsync(opts.Id);

            // Ensure the entity exists
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
                    ? Permissions.EditOwnTopics
                    : Permissions.EditAnyTopic))
            {
                return Unauthorized();
            }

            // Add entity we are editing to context
            HttpContext.Items[typeof(Topic)] = entity;

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                        .Action("Index", "Home", "Plato.Core")
                        .LocalNav()
                    ).Add(S["Discuss"], discuss => discuss
                        .Action("Index", "Home", "Plato.Discuss")
                        .LocalNav()
                    ).Add(S[entity.Title.TrimToAround(75)], post => post
                        .Action("Display", "Home", "Plato.Discuss", new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Topic"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View((LayoutViewModel) await _entityViewProvider.ProvideEditAsync(entity, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditEntityViewModel viewModel)
        {

            // Get entity we are editing 
            var entity = await _entityStore.GetByIdAsync(viewModel.Id);

            // Ensure entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We always need to be logged in to edit entities
            if (user == null)
            {
                return Unauthorized();
            }

            // Only update edited information if the message changes
            if (viewModel.Message != entity.Message)
            {
                entity.EditedUserId = user?.Id ?? 0;
                entity.EditedDate = DateTimeOffset.UtcNow;
            }

            //// Update title & message
            entity.Title = viewModel.Title;
            entity.Message = viewModel.Message;

            // Validate model state within all view providers
            if (await _entityViewProvider.IsModelStateValidAsync(entity, this))
            {
                
                // Always update modified information
                entity.ModifiedUserId = user?.Id ?? 0;
                entity.ModifiedDate = DateTimeOffset.UtcNow;

                // Get composed model from view providers
                entity = await _entityViewProvider.ComposeModelAsync(entity, this);

                // Update the entity
                var result = await _topicManager.UpdateAsync(entity);

                // Ensure success
                if (result.Succeeded)
                {

                    // Execute view providers ProvideUpdateAsync method
                    await _entityViewProvider.ProvideUpdateAsync(result.Response, this);

                    // Get authorize result
                    var authorizeResult = await AuthorizeAsync(result.Response);
                    if (authorizeResult.Succeeded)
                    {

                        // Everything was OK
                        _alerter.Success(T["Topic Updated Successfully!"]);

                        // Redirect to entity
                        return RedirectToAction(nameof(Display), new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        });

                    }

                    // Add any authorization errors
                    foreach (var error in authorizeResult.Errors)
                    {
                        _alerter.Success(T[error.Description]);
                    }

                    // Redirect to index
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }

            return await Edit(new EntityOptions()
            {
                Id = entity.Id,
                Alias = entity.Alias
            });

        }

        // -----------------
        // Edit Entity Reply
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
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Get current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Do we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
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
                    ).Add(S[entity.Title.TrimToAround(75)], post => post
                        .Action("Display", "Home", "Plato.Discuss", new RouteValueDictionary()
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias
                        })
                        .LocalNav()
                    )
                    .Add(S["Edit Reply"], post => post
                        .LocalNav()
                    );
            });

            // Return view
            return View((LayoutViewModel) await _replyViewProvider.ProvideEditAsync(reply, this));

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
            if (await _replyViewProvider.IsModelStateValidAsync(reply, this))
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
            return Task.FromResult((IActionResult) View(viewModel));

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
            Reply reply = null;
            if (model.Options.ReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(model.Options.ReplyId);
                if (reply == null)
                {
                    return NotFound();
                }
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Invoke report manager and compile results
            if (reply != null)
            {
                // Report reply
                await _reportReplyManager.ReportAsync(new ReportSubmission<Reply>()
                {
                    Who = user,
                    What = reply,
                    Why = (ReportReasons.Reason) model.ReportReason
                });
            }
            else
            {
                // Report entity
                await _reportEntityManager.ReportAsync(new ReportSubmission<Topic>()
                {
                    Who = user,
                    What = entity,
                    Why = (ReportReasons.Reason) model.ReportReason
                });
            }

            _alerter.Success(reply != null
                ? T["Thank You. Reply Reported Successfully!"]
                : T["Thank You. Topic Reported Successfully!"]);

            // Redirect
            return RedirectToAction(nameof(Reply), new RouteValueDictionary()
            {
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            });

        }

        // -----------------
        // Display Reply
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

            // Configure options
            opts = ConfigureEntityDisplayOptions(entity, opts);

            // Get offset for given reply
            var offset = 0;
            if (opts.ReplyId > 0)
            {
                // We need to iterate all replies to calculate the offset
                var replies = await _replyService
                    .ConfigureQuery(async q =>
                    {

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Permissions.ViewHiddenReplies))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Permissions.ViewSpamReplies))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                            Permissions.ViewDeletedReplies))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync(opts, new PagerOptions
                {
                    Size = int.MaxValue
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
                    ["area"] = "Plato.Discuss",
                    ["controller"] = "Home",
                    ["action"] = "Display",
                    ["opts.id"] = entity.Id,
                    ["opts.alias"] = entity.Alias
                }));
            }

            // Redirect to offset within entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["pager.offset"] = offset,
            }));

        }

        // -----------------
        // Entity Helpers
        // -----------------
        
        public async Task<IActionResult> Pin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.PinTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update topic
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsPinned = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Pinned Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove topic from SPAM"]);
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

        public async Task<IActionResult> Unpin(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.UnpinTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsPinned = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Pin Removed Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove pin"]);
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

        public async Task<IActionResult> Hide(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.HideTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsHidden = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Hidden Successfully"]);

                if (result.Response.IsHidden)
                {
                    // Do we have permission to view hidden entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewHiddenTopics))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Discuss",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }

            }
            else
            {
                _alerter.Danger(T["Could not hide the topic"]);
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

        public async Task<IActionResult> Show(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ShowTopics))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsHidden = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Made Public Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not update the topic"]);
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

        public async Task<IActionResult> Lock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out int entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.LockTopics))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsLocked = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Locked Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not lock the topic"]);
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

        public async Task<IActionResult> Unlock(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.UnlockTopics))
            {
                return Unauthorized();
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsLocked = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Unlocked Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not open the topic"]);
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

        public async Task<IActionResult> ToSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.TopicToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsSpam = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Marked as SPAM"]);
                
                if (result.Response.IsSpam)
                {
                    // Do we have permission to view spam entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewSpamTopics))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Discuss",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }

            }
            else
            {
                _alerter.Danger(T["Could not mark topic as SPAM"]);
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

        public async Task<IActionResult> FromSpam(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
            if (!ok)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.TopicFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsSpam = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);

            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Removed from SPAM"]);
            }
            else
            {
                _alerter.Danger(T["Could not remove topic from SPAM"]);
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
        
        public async Task<IActionResult> Delete(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
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

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.DeleteOwnTopics
                    : Permissions.DeleteAnyTopic))
            {
                return Unauthorized();
            }

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsDeleted = true;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Deleted Successfully"]);

                if (result.Response.IsDeleted)
                {
                    // Do we have permission to view deleted entities
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        entity.CategoryId, Permissions.ViewDeletedTopics))
                    {
                        // Redirect to index
                        return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
                        {
                            ["area"] = "Plato.Discuss",
                            ["controller"] = "Home",
                            ["action"] = "Index"
                        }));
                    }
                }

            }
            else
            {
                _alerter.Danger(T["Could not delete the topic"]);
            }
            
    
            // Else redirect back to entity
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias
            }));

        }
 
        public async Task<IActionResult> Restore(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
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

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.RestoreOwnTopics
                    : Permissions.RestoreAnyTopic))
            {
                return Unauthorized();
            }

            // Update entity
            entity.ModifiedUserId = user?.Id ?? 0;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.IsDeleted = false;

            // Save changes and return results
            var result = await _topicManager.UpdateAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the topic"]);
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

        public async Task<IActionResult> PermanentDelete(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var entityId);
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

            // Get entity
            var entity = await _entityStore.GetByIdAsync(entityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == entity.CreatedUserId
                    ? Permissions.PermanentDeleteOwnTopics
                    : Permissions.PermanentDeleteAnyTopic))
            {
                return Unauthorized();
            }

            // Delete entity
            var result = await _topicManager.DeleteAsync(entity);
            if (result.Succeeded)
            {
                _alerter.Success(T["Topic Permanently Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not permanently delete the topic"]);
            }

            // Redirect back to index
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Index"
            }));

        }
        
        // -----------------
        // Entity Reply Helpers
        // -----------------

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

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.HideReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
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
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
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

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ShowReplies))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
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
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
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

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ReplyToSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
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
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
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

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure the entity exists
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, entity.CategoryId, Permissions.ReplyFromSpam))
            {
                return Unauthorized();
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update entity
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
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
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

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
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
                _alerter.Success(T["Reply Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not delete the reply"]);
            }

            // Redirect back to entity
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
        
        public async Task<IActionResult> RestoreReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
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

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
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
                _alerter.Success(T["Reply Restored Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not restore the reply"]);
            }

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

        public async Task<IActionResult> PermanentDeleteReply(string id)
        {

            // Ensure we have a valid id
            var ok = int.TryParse(id, out var replyId);
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

            // Ensure the entity exists
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return NotFound();
            }

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(this.User, entity.CategoryId,
                user.Id == reply.CreatedUserId
                    ? Permissions.PermanentDeleteOwnReplies
                    : Permissions.PermanentDeleteAnyReply))
            {
                return Unauthorized();
            }

            // Delete reply
            var result = await _replyManager.DeleteAsync(reply);
            if (result.Succeeded)
            {
                _alerter.Success(T["Reply Permanently Deleted Successfully"]);
            }
            else
            {
                _alerter.Danger(T["Could not permanently delete the reply"]);
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

        #endregion

        #region "Private Methods"

        async Task<ICommandResultBase> AuthorizeAsync(IEntity entity)
        {

            // Our result
            var result = new CommandResultBase();

            // Generic error message
            const string error = "Topic added but pending approval";
         
            // IsHidden
            if (entity.IsHidden)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewHiddenTopics))
                {
                    return result.Failed(error);
                }
            }

            // IsSpam
            if (entity.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewSpamTopics))
                {
                    return result.Failed(error);
                }
            }

            // IsDeleted
            if (entity.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewDeletedTopics))
                {
                    return result.Failed(error);
                }
            }

            return result.Success();

        }
        
        async Task<ICommandResultBase> AuthorizeAsync(IEntityReply reply)
        {

            // Our result
            var result = new CommandResultBase();

            // Get entity
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);

            // Ensure entity exists
            if (entity == null)
            {
                return result.Failed("The topic has since been deleted!");
            }

            // Generic failure message
            const string error = "Reply added but pending approval";

            // IsHidden
            if (reply.IsHidden)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewHiddenReplies))
                {
                    return result.Failed(error);
                }
            }

            // IsSpam
            if (reply.IsSpam)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewSpamReplies))
                {
                    return result.Failed(error);
                }
            }

            // IsDeleted
            if (reply.IsDeleted)
            {
                if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                    entity.CategoryId, Permissions.ViewDeletedReplies))
                {
                    return result.Failed(error);
                }
            }

            return result.Success();

        }

        async Task<EntityIndexViewModel<Topic>> GetIndexViewModelAsync(EntityIndexOptions options, PagerOptions pager)
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

            // Ensure pinned appear first
            if (options.Sort == SortBy.LastReply)
            {
                options.AddSortColumn(SortBy.IsPinned.ToString(), OrderBy.Desc);
            }
            
            // Return updated model
            return new EntityIndexViewModel<Topic>()
            {
                Options = options,
                Pager = pager
            };

        }

        EntityViewModel<Topic, Reply> GetDisplayViewModel(Topic entity, EntityOptions options, PagerOptions pager)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Configure options
            options = ConfigureEntityDisplayOptions(entity, options);

            // Return updated model
            return new EntityViewModel<Topic, Reply>()
            {
                Entity = entity,
                Options = options,
                Pager = pager
            };
        }
        
        EntityOptions ConfigureEntityDisplayOptions(Topic entity, EntityOptions options)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Ensure view model is aware of the entity we are displaying
            options.Id = entity.Id;

            // Ensure replies marked as an answer appear first
            options.SortColumns.Add("IsAnswer", OrderBy.Desc);
            options.SortColumns.Add("CreatedDate", OrderBy.Asc);

            return options;

        }

        IEnumerable<SelectListItem> GetReportReasons()
        {

            var output = new List<SelectListItem>();
            foreach (var reason in ReportReasons.Reasons)
            {
                output.Add(new SelectListItem
                {
                    Text = S[reason.Value],
                    Value = Convert.ToString((int) reason.Key)
                });
            }

            return output;
        }

        // ------------

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
            var feature = await _featureFacade.GetFeatureByIdAsync(RouteData.Values["area"].ToString());

            var entity = new Topic()
            {
                Title = "Test Topic " + rnd.Next(0, 2000).ToString(),
                Message = GetSampleMarkDown(rnd.Next(0, 2000)),
                FeatureId = feature?.Id ?? 0,
                CreatedUserId = randomUser?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // create topic
            var data = await _topicManager.CreateAsync(entity);
            if (data.Succeeded)
            {
                for (var i = 0; i < 28; i++)
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
