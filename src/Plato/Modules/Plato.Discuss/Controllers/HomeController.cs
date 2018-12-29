using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Internal.Navigation;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
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

        private readonly IPlatoUserStore<User> _ploatUserStore;

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
            IPlatoUserStore<User> ploatUserStore)
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
            _ploatUserStore = ploatUserStore;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        // -----------------
        // Topics
        // -----------------

        public async Task<IActionResult> Index(
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

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"]);
            });

            //await CreateSampleData();

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

            // Build infinate scroll options
            opts.InfiniteScrollOptions = GetIndexInfiniteScrollOptions();

            // Build view model
            var viewModel = new TopicIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            this.HttpContext.Items[typeof(TopicIndexViewModel)] = viewModel;

            // Return simple paged results for ajax callback
            if (pager.Page > 1)
            {
                return View("GetTopics", viewModel);
            }

            // Return view
            return View(await _topicViewProvider.ProvideIndexAsync(new Topic(), this));

        }

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

            return Index(opts, pager);
        }

        // -----------------
        // New topic
        // -----------------

        public async Task<IActionResult> Create(int channel)
        {
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
                ).Add(S["Post Topic"], post => post
                    .LocalNav()
                );
            });

            var result = await _topicViewProvider.ProvideEditAsync(topic, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditTopicViewModel model)
        {

            // Validate model state within all view providers
            if (await _topicViewProvider.IsModelStateValid(new Topic()
            {
                Title = model.Title,
                Message = model.Message
            }, this))
            {

                // Get composed type from all involved view providers
                var topic = await _topicViewProvider.GetComposedType(this);

                // Populated created by
                var user = await _contextFacade.GetAuthenticatedUserAsync();
                topic.CreatedUserId = user?.Id ?? 0;
                topic.CreatedDate = DateTimeOffset.UtcNow;

                // We need to first add the fully composed type
                // so we have a nuique entity Id for all ProvideUpdateAsync
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
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create(0);

        }

        // -----------------
        // display topic
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

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], discuss => discuss
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                ).Add(S[topic.Title], post => post
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

            // Build infinate scroll options
            opts.InfiniteScrollOptions = GetTopicInfiniteScrollOptions(topic);

            // Build view model
            var viewModel = new TopicViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add models to context for use within view adaptors
            this.HttpContext.Items[typeof(TopicViewModel)] = viewModel;
            this.HttpContext.Items[typeof(Topic)] = topic;

            // Return paged results for ajax callback
            if (pager.Page > 1)
            {
                return View("GetTopicReplies", viewModel);
            }

            // Return view
            return View(await _topicViewProvider.ProvideDisplayAsync(topic, this));

        }

        // -----------------
        // post new reply
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

                // We need to first add the reply so we have a nuique Id
                // for all ProvideUpdateAsync methods within any involved view provider
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


        // edit topic

        public async Task<IActionResult> Edit(int id)
        {

            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            var result = await _topicViewProvider.ProvideEditAsync(topic, this);

            // Return view
            return View(result);

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

                // Update title & message
                topic.Title = model.Title;
                topic.Message = model.Message;

                // Populated created by
                var user = await _contextFacade.GetAuthenticatedUserAsync();
                topic.ModifiedUserId = user?.Id ?? 0;
                topic.ModifiedDate = DateTimeOffset.UtcNow;

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

        // edit reply

        public async Task<IActionResult> EditReply(int id)
        {

            var reply = await _entityReplyStore.GetByIdAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

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

            // Update the message
            reply.Message = model.Message;
            reply.ModifiedUserId = user?.Id ?? 0;
            reply.ModifiedDate = DateTimeOffset.UtcNow;

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

        public Task<IActionResult> Report(
            int entityId,
            int entityReplyId = 0)
        {


            // Return view
            return Task.FromResult((IActionResult) View());

        }

        #endregion

        #region "Private Methods"

        InfiniteScrollOptions GetIndexInfiniteScrollOptions()
        {

            // Swaps current action for "Get" action and removes current page 
            var routeValues = new RouteValueDictionary(this.RouteData.Values);
            routeValues.Remove("pager.page");
            routeValues.Remove("offset");

            return new InfiniteScrollOptions()
            {
                CallbackUrl = _contextFacade.GetRouteUrl(routeValues)
            };

        }

        InfiniteScrollOptions GetTopicInfiniteScrollOptions(Topic topic)
        {

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            
            RouteData.Values.Remove("pager.page");
            RouteData.Values.Remove("offset");

            var url = _contextFacade.GetRouteUrl(RouteData.Values);
            return new InfiniteScrollOptions()
            {
                CallbackUrl = url
            };

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

            var users = await _ploatUserStore.QueryAsync()
                .Take(1, 1000)
                .Select<UserQueryParams>(q => { })
                .OrderBy("LastLoginDate", OrderBy.Desc)
                .ToList();

            var rnd = new Random();
            var topic = new Topic()
            {
                Title = "Test Topic ♥♥♥ " + rnd.Next(0, users.Total).ToString(),
                Message = GetSampleMarkDown(rnd.Next(0, users.Total)),
                CreatedUserId = users.Data[rnd.Next(0, users.Total)].Id,
                CreatedDate = DateTimeOffset.UtcNow
            };

            // create topic
            var data = await _topicManager.CreateAsync(topic);
            if (data.Succeeded)
            {

                for (var i = 0; i < 100; i++)
                {
                    rnd = new Random();
                    var reply = new Reply()
                    {
                        EntityId = data.Response.Id,
                        Message = GetSampleMarkDown(i) + " - reply: " + i.ToString(),
                        CreatedUserId = users.Data[rnd.Next(0, users.Total)].Id,
                        CreatedDate = DateTimeOffset.UtcNow
                    };
                    var newReply = await _replyManager.CreateAsync(reply);
                }



            }

        }

        #endregion

    }

}
