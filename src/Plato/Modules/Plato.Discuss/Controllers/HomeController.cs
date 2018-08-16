using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"
        
        private readonly IViewProviderManager<Topic> _topicViewProvider;
        private readonly IViewProviderManager<Reply> _replyViewProvider;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IPostManager<Topic> _postManager;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

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
            IPostManager<Topic> postManager,
            IPostManager<Reply> replyManager,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager)
        {
            _topicViewProvider = topicViewProvider;
            _replyViewProvider = replyViewProvider;
            _entityStore = entityStore;
            _entityReplyStore = entityReplyStore;
            _postManager = postManager;
            _replyManager = replyManager;
            _alerter = alerter;
            _breadCrumbManager = breadCrumbManager;

            T = localizer;
            S = stringLocalizer;

        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            
            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
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

            //this.RouteData.Values.Add("Options.Search", filterOptions.Search);
            //this.RouteData.Values.Add("Options.Order", filterOptions.Order);
            this.RouteData.Values.Add("page", pagerOptions.Page);
         
            // Build view
            var result = await _topicViewProvider.ProvideIndexAsync(new Topic(), this);

            // Return view
            return View(result);
            
        }
        
        // add new topic

        public async Task<IActionResult> Create(int channel)
        {
            var topic = new Topic();
            if (channel > 0)
            {
                topic.CategoryId = channel;
            }

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

                // We need to first add the entity so we have a nuique entity Id
                // for all ProvideUpdateAsync methods within any involved view provider
                var topic = await _postManager.CreateAsync(new Topic()
                {
                    Title = model.Title,
                    Message = model.Message
                });

                // Ensure the insert was successful
                if (topic.Succeeded)
                {

                    // Execute view providers ProvideUpdateAsync method
                    await _topicViewProvider.ProvideUpdateAsync(topic.Response, this);

                    // Everything was OK
                    _alerter.Success(T["Topic Created Successfully!"]);

                    // Redirect to topic
                    return RedirectToAction(nameof(Topic), new {Id = topic.Response.Id});
                    
                }
                else
                {
                    // Errors that may have occurred whilst creating the entity
                    foreach (var error in topic.Errors)
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

        // display topic

        public async Task<IActionResult> Topic(
            int id,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            
            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }


            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }
            
            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Search", filterOptions.Search);
            routeData.Values.Add("Options.Order", filterOptions.Order);
            routeData.Values.Add("page", pagerOptions.Page);
            

            // Build view
            var result = await _topicViewProvider.ProvideDisplayAsync(topic, this);

            // Return view
            return View(result);
            
        }
        
        // reply to topic

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
            
            // Validate model state within all view providers
            if (await _replyViewProvider.IsModelStateValid(new Reply()
            {
                Id = model.EntityId,
                Message = model.Message
            }, this))
            {

                // We need to first add the reply so we have a nuique Id
                // for all ProvideUpdateAsync methods within any involved view provider
                var reply = await _replyManager.CreateAsync(new Reply()
                {
                    EntityId = model.EntityId,
                    Message = model.Message
                });

                // Ensure the insert was successful
                if (reply.Succeeded)
                {

                    // Execute view providers ProvideUpdateAsync method
                    await _replyViewProvider.ProvideUpdateAsync(reply.Response, this);

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
                    foreach (var error in reply.Errors)
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

            return await Topic(topic.Id, null, null);
            
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
        
        #endregion

        #region "Private Methods"

        private async Task<string> CreateSampleData()
        { 

            var rnd = new Random();
            var topic = new Topic()
            {
                Title = "Test Topic " + rnd.Next(0, 100000).ToString(),
                Message = @"Hi There, 

# header 1

Test message Test message Test message Test :)

## Header 2

message Test message Test message Test message :(

Test message Test message Test message Test 

    var entity = await _entityStore.GetByIdAsync(entityId);
    var replies = await GetEntityReplies(entityId, filterOptions, pagerOptions);
        return new HomeTopicViewModel(
            entity,
            replies,
            filterOptions,
            pagerOptions);

### Header 3

message Test message Test message Test message 

Test message Test message Test message Test message 

#### Header 4

Test message Test message Test message Test message Test 

- list 1
- list 2
- list 3

message Test message  " + rnd.Next(0, 100000).ToString(),
};

            var topicDetails = new PostDetails()
            {
                SomeNewValue = "Example Value 123",
                Participants = new List<SimpleUser>()
                            {
                                new SimpleUser()
                                {
                                    Id = 1,
                                    UserName = "Test"

                                },
                                new SimpleUser()
                                {
                                    Id = 2,
                                    UserName = "Mike Jones"
                                },
                                new SimpleUser()
                                {
                                    Id = 3,
                                    UserName = "Sarah Smith"
                                },
                                new SimpleUser()
                                {
                                    Id = 4,
                                    UserName = "Mark Williams"
                                },
                                new SimpleUser()
                                {
                                    Id = 5,
                                    UserName = "Marcus"
                                }
                            }
            };

            topic.AddOrUpdate<PostDetails>(topicDetails);

            var sb = new StringBuilder();

            var data = await _postManager.CreateAsync(topic);
            if (data.Succeeded)
            {
                if (data.Response is Entity newTopic)
                {

                    sb
                        .Append("<h1>New Topic</h1>")
                        .Append("<strong>Title</strong>")
                        .Append("<br>")
                        .Append(newTopic.Title)
                        .Append("<br>")
                        .Append("<strong>ID</strong>")
                        .Append(newTopic.Id);

                    var details = newTopic.GetOrCreate<PostDetails>();
                    if (details?.Participants != null)
                    {

                        sb.Append("<h5>Some Value</h5>")
                            .Append(details.SomeNewValue)
                            .Append("<br>");

                        sb.Append("<h5>Participants</h5>");

                        foreach (var user in details.Participants)
                        {
                            sb.Append(user.UserName)
                                .Append("<br>");
                        }


                    }
                }
            }

            var existingTopic = await _entityStore.GetByIdAsync(142);
            if (existingTopic != null)
            {

                sb
                    .Append("<h1>Existing Topic</h1>")
                    .Append("<strong>Title </strong>")
                    .Append("<br>")
                    .Append(existingTopic.Title)
                    .Append("<br>")
                    .Append("<strong>ID </strong>")
                    .Append(existingTopic.Id);

                // random details
                var existingDetails = existingTopic.GetOrCreate<PostDetails>();
                if (existingDetails?.Participants != null)
                {

                    sb.Append("<h5>Some Value</h5>")
                        .Append(existingDetails.SomeNewValue)
                        .Append("<br>");

                    sb.Append("<h5>Participants</h5>");

                    foreach (var user in existingDetails.Participants)
                    {
                        sb.Append(user.UserName)
                            .Append("<br>");
                    }

                }

            }


            return sb.ToString();

        }

        #endregion
        
    }
    
}
