using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<HomeIndexViewModel> _homeIndexViewProvider;
        private readonly IViewProviderManager<HomeTopicViewModel> _homeTopicViewProvider;

        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        
        private readonly IEntityStore<Entity> _entityStore;


        private readonly IEntityManager<Entity> _entityManager;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IAlerter _alerter;
        
        public IHtmlLocalizer T { get; }



        public HomeController(
            IHtmlLocalizer<HomeController> localizer,
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore,
            IEntityReplyStore<EntityReply> entityReplyStore,
            IViewProviderManager<HomeIndexViewModel> homeIndexViewProvider,
            IViewProviderManager<HomeTopicViewModel> homeTopicViewProvider,
            IEntityManager<Entity> entityManager,
            IAlerter alerter)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _entityManager = entityManager;
            _entityStore = entityStore;
            _entityReplyStore = entityReplyStore;
            _homeIndexViewProvider = homeIndexViewProvider;
            _homeTopicViewProvider = homeTopicViewProvider;
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

            //var feature = await _contextFacade.GetCurrentFeatureAsync();

            //ViewBag.Feature = feature;

            // ------------------------

//            var rnd = new Random();

//            var topic = new Entity()
//            {
//                Title = "Test Topic " + rnd.Next(0, 100000).ToString(),
//                Message = @"Hi There, 

//# header 1

//Test message Test message Test message Test :)

//## Header 2

//message Test message Test message Test message :(

//Test message Test message Test message Test 

//message Test message Test message Test message 

//Test message Test message Test message Test message 

//Test message Test message Test message Test message Test 

//- list 1
//- list 2
//- list 3

//message Test message  " + rnd.Next(0, 100000).ToString(),
//            };

//            var topicDetails = new EntityMetaData()
//            {
//                SomeNewValue = "Example Value 123",
//                Users = new List<Participant>()
//                {
//                    new Participant()
//                    {
//                        UserId = 1,
//                        UserName = "Test",
//                        Participations = 10

//                    },
//                    new Participant()
//                    {
//                        UserId = 2,
//                        UserName = "Mike Jones",
//                        Participations = 5
//                    },
//                    new Participant()
//                    {
//                        UserId = 3,
//                        UserName = "Sarah Smith",
//                        Participations = 2
//                    }
//                }
//            };

//            topic.SetMetaData<EntityMetaData>(topicDetails);

//            var sb = new StringBuilder();

//            var data = await _entityManager.CreateAsync(topic);
//            if (data.Succeeded)
//            {
//                if (data.Response is Entity newTopic)
//                {

//                    sb
//                        .Append("<h1>New Topic</h1>")
//                        .Append("<strong>Title</strong>")
//                        .Append("<br>")
//                        .Append(newTopic.Title)
//                        .Append("<br>")
//                        .Append("<strong>ID</strong>")
//                        .Append(newTopic.Id);

//                    var details = newTopic.GetMetaData<EntityMetaData>();
//                    if (details?.Users != null)
//                    {

//                        sb.Append("<h5>Some Value</h5>")
//                            .Append(details.SomeNewValue)
//                            .Append("<br>");

//                        sb.Append("<h5>Participants</h5>");

//                        foreach (var user in details.Users)
//                        {
//                            sb.Append(user.UserName)
//                                .Append("<br>");
//                        }


//                    }
//                }
//            }

//            var existingTopic = await _entityStore.GetByIdAsync(142);
//            if (existingTopic != null)
//            {

//                sb
//                    .Append("<h1>Existing Topic</h1>")
//                    .Append("<strong>Title </strong>")
//                    .Append("<br>")
//                    .Append(existingTopic.Title)
//                    .Append("<br>")
//                    .Append("<strong>ID </strong>")
//                    .Append(existingTopic.Id);

//                // random details
//                var existingDetails = existingTopic.GetMetaData<EntityMetaData>();
//                if (existingDetails?.Users != null)
//                {

//                    sb.Append("<h5>Some Value</h5>")
//                        .Append(existingDetails.SomeNewValue)
//                        .Append("<br>");

//                    sb.Append("<h5>Participants</h5>");

//                    foreach (var user in existingDetails.Users)
//                    {
//                        sb.Append(user.UserName)
//                            .Append("<br>");
//                    }

//                }

//            }


//            ViewBag.TopicData = sb.ToString();

            // ------------------------



            //var entityDetails1 = new EntityDetails()
            //{
            //    SomeValue = "entityDetails1"
            //};

            //var entityDetails2 = new EntityDetails()
            //{
            //    SomeValue = "entityDetails2"
            //};

            //var entity = new Entity()
            //{
            //    FeatureId = feature.Id,
            //    Title = "Test Topic " + rnd.Next(0, 100000).ToString(),
            //    Markdown = "Test message " + rnd.Next(0, 100000).ToString(),
            //    Html = "Test message " + rnd.Next(0, 100000).ToString(),
            //    PlainText = "Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message Test message  " + rnd.Next(0, 100000).ToString(),
            //    Data = new List<EntityData>()
            //    {
            //        new EntityData()
            //        {
            //            Key = "Data1",
            //            Value = entityDetails1.Serialize()
            //        },
            //        new EntityData()
            //        {
            //            Key = "Data2",
            //            Value = entityDetails2.Serialize()
            //        }
            //    }

            //};

            //var newEntity = await _entityStore.CreateAsync(entity);

            //var pagerOptions = new PagerOptions()
            //{
            //    Page = 1,
            //    PageSize = 20
            //};

            //var model = new DiscussIndexViewModel()
            //{
            //    Results = await GetEntities(pagerOptions)
            //};

            //return View(model);


            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Search", filterOptions.Search);
            routeData.Values.Add("Options.Order", filterOptions.Order);
            
            // Get model
            var model = await GetIndexViewModel(filterOptions, pagerOptions);

            // Build view
            var result = await _homeIndexViewProvider.ProvideIndexAsync(model, this);

            // Return view
            return View(result);
            

        }
        
        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(HomeIndexViewModel model)
        {

       
            var result = await _homeIndexViewProvider.ProvideUpdateAsync(model, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }
            
            _alerter.Success(T["Topic Created Successfully!"]);

            return RedirectToAction(nameof(Index));
            

            //topic.SetMetaData<TopicDetails>(topicDetails);

            //var sb = new StringBuilder();

            //var newTopic = await _entityStore.CreateAsync(topic);
            
            //return View(model);

        }
        
        public async Task<IActionResult> Channel()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public async Task<IActionResult> Topic(
            int id,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            var entity = await _entityStore.GetByIdAsync(id);
            if (entity == null)
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
            
            // Get model
            var model = await GetTopicViewModel(id, filterOptions, pagerOptions);
            
            // Build view
            var result = await _homeTopicViewProvider.ProvideIndexAsync(model, this);

            // Return view
            return View(result);
            
        }
        
        [HttpPost]
        [ActionName(nameof(Topic))]
        public async Task<IActionResult> TopicPost(int id)
        {
        
            var entity = await _entityStore.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }


            var model = new HomeTopicViewModel()
            {
                Entity = entity
            };

            var result = await _homeTopicViewProvider.ProvideUpdateAsync(model, this);

            if (!ModelState.IsValid)
            {
                return View(result);
            }

            _alerter.Success(T["Reply Added Successfully!"]);

            return RedirectToAction(nameof(Topic));
            
        }


        #endregion

        #region "Private Methods"

        private async Task<HomeIndexViewModel> GetIndexViewModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var topics = await GetEntities(filterOptions, pagerOptions);
            return new HomeIndexViewModel(
                topics,
                filterOptions,
                pagerOptions);
        }

        private async Task<HomeTopicViewModel> GetTopicViewModel(
            int entityId,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var entity = await _entityStore.GetByIdAsync(entityId);
            var replies = await GetEntityReplies(entityId, filterOptions, pagerOptions);
            return new HomeTopicViewModel(
                entity,
                replies,
                filterOptions,
                pagerOptions);
        }
        
        public async Task<IPagedResults<Entity>> GetEntities(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            // Get current feature (i.e. Plato.Discuss) from area
            var feature = await _contextFacade.GetCurrentFeatureAsync();

            return await _entityStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityQueryParams>(q =>
                {

                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();

                    //q.IsPinned.True();


                    //if (!string.IsNullOrEmpty(filterOptions.Search))
                    //{
                    //    q.UserName.IsIn(filterOptions.Search).Or();
                    //    q.Email.IsIn(filterOptions.Search);
                    //}
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }
        
        public async Task<IPagedResults<EntityReply>> GetEntityReplies(
            int entityId,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _entityReplyStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                    //q.IsPrivate();
                    //q.IsSpam.False();
                    //q.IsDeleted.False();
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.Keywords.IsIn(filterOptions.Search);
                    }
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }

        #endregion



    }



}
