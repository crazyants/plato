using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
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
        
        private readonly IViewProviderManager<Topic> _discussViewProvider;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IEntityStore<Topic> _entityStore;
      
        private readonly IPostManager<Topic> _postManager;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }
        
        public HomeController(
            IHtmlLocalizer<HomeController> localizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IEntityStore<Topic> entityStore,
            IViewProviderManager<Topic> discussViewProvider,
            IPostManager<Topic> postManager,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _postManager = postManager;
            _entityStore = entityStore;
            _discussViewProvider = discussViewProvider;
            _alerter = alerter;
            T = localizer;
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

            await CreateSampleData();

            //this.RouteData.Values.Add("Options.Search", filterOptions.Search);
            //this.RouteData.Values.Add("Options.Order", filterOptions.Order);
            this.RouteData.Values.Add("page", pagerOptions.Page);
         
            // Build view
            var result = await _discussViewProvider.ProvideIndexAsync(new Topic(), this);

            // Return view
            return View(result);
            

        }

        public async Task<IActionResult> Create()
        {
            
            var result = await _discussViewProvider.ProvideEditAsync(new Topic(), this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(EditEntityViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate model state within all view providers
            if (await _discussViewProvider.IsModelStateValid(new Topic()
            {
                Title = model.Title,
                Message = model.Message
            }, this))
            {

                // We need to first add the entity so we have a nuique entity Id
                // for all ProvideUpdateAsync methods within each view provider
                var topic = await _postManager.CreateAsync(new Topic()
                {
                    Title = model.Title,
                    Message = model.Message
                });

                // Ensure the insert was successful
                if (topic.Succeeded)
                {

                    // Execute view providers
                    var result = await _discussViewProvider.ProvideUpdateAsync(topic.Response, this);

                    // Everything was OK
                    _alerter.Success(T["Topic Created Successfully!"]);

                    // 
                    return View(result);

                }
                
            }


            // if we reach this point some view model validation
            // within a view providers failed, display errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Create();





            //var result = await _discussViewProvider.ProvideUpdateAsync(new Topic()
            //{
            //    Title = model.Title,
            //    Message = model.Message
            //}, this);

            //// Ensure model state is still validate after executing all view providers
            //if (this.ModelState.IsValid)
            //{
            //    // Everything was OK
            //    _alerter.Success(T["Topic Created Successfully!"]);
            //    return View(result);
            //}







            //// We need to first add the entity so we have a nuique entity Id
            //// for all ProvideUpdateAsync methods within each view provider
            //var topic = await _postManager.CreateAsync(new Topic()
            //{
            //    Title = model.Title,
            //    Message = model.Message
            //});

            //// Ensure the insert was successful
            //if (topic.Succeeded)
            //{

            //    // Execute view providers
            //    var result = await _discussViewProvider.ProvideUpdateAsync(topic.Response, this);

            //    // Ensure model state is still validate after executing all view providers
            //    if (this.ModelState.IsValid)
            //    {
            //        // Everything was OK
            //        _alerter.Success(T["Topic Created Successfully!"]);
            //        return View(result);
            //    }

            //    // if we reach this point some view model validation
            //    // within a view providers ProvideUpdateAsync method failed
            //    foreach (var modelState in ViewData.ModelState.Values)
            //    {
            //        foreach (var error in modelState.Errors)
            //        {
            //            _alerter.Danger(T[error.ErrorMessage]);
            //        }
            //    }

            //    return await Create();

            //}
            //else
            //{

            //    // We could not add the initial entity
            //    _alerter.Danger(T["A problem occurred creating the topic. Please try again.!"]);
            //    return await Create();

            //}

        }

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
            var result = await _discussViewProvider.ProvideDisplayAsync(topic, this);

            // Return view
            return View(result);
            
        }
        
        [HttpPost]
        [ActionName(nameof(Topic))]
        public async Task<IActionResult> TopicPost(int id)
        {

            var topic = await _entityStore.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            
            var result = await _discussViewProvider.ProvideUpdateAsync(topic, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Reply Added Successfully!"]);

            return RedirectToAction(nameof(Topic));
            
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
