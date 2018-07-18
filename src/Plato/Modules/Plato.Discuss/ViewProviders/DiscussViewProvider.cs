using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewProviders
{
    public class DiscussViewProvider : BaseViewProvider<Topic>
    {

        private const string EditorHtmlName = "message";

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;

        private readonly IPostManager<Topic> _topicManager;
        private readonly IPostManager<EntityReply> _replyManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly HttpRequest _request;
        
        public DiscussViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IPostManager<EntityReply> replyManager,
            IEntityReplyStore<EntityReply> entityReplyStore,
            IActionContextAccessor actionContextAccessor,
            IContextFacade contextFacade,
            IEntityStore<Topic> entityStore,
            IPostManager<Topic> topicManager)
        {
            _replyManager = replyManager;
            _entityReplyStore = entityReplyStore;
            _actionContextAccessor = actionContextAccessor;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _topicManager = topicManager;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic topic, IUpdateModel updater)
        {

            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = GetPageIndex(updater);
            
            var indexViewModel = new HomeIndexViewModel();
            indexViewModel.FilterOpts = filterOptions;
            indexViewModel.PagerOpts = pagerOptions;


                //await GetIndexViewModel(filterOptions, pagerOptions);

            return Views(
                View<HomeIndexViewModel>("Home.Index.Header", model => indexViewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Index.Tools", model => new EditEntityViewModel()
                {
                    EditorHtmlName = EditorHtmlName
                }).Zone("tools"),
                View<HomeIndexViewModel>("Home.Index.Sidebar", model => indexViewModel).Zone("sidebar"),
                View<HomeIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content")
            );

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic viewModel, IUpdateModel updater)
        {
           
            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = GetPageIndex(updater);


            var topic = await _entityStore.GetByIdAsync(viewModel.Id);
            if (topic == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }


            var replies = await GetEntityReplies(topic.Id, filterOptions, pagerOptions);
            

            var topivViewModel = new HomeTopicViewModel(replies, pagerOptions)
            {
                Entity = topic
            };

            return Views(
                View<HomeTopicViewModel>("Home.Topic.Header", model => topivViewModel).Zone("header"),
                View<HomeTopicViewModel>("Home.Topic.Tools", model => topivViewModel).Zone("tools"),
                View<HomeTopicViewModel>("Home.Topic.Sidebar", model => topivViewModel).Zone("sidebar"),
                View<HomeTopicViewModel>("Home.Topic.Content", model => topivViewModel).Zone("content"),
                View<NewEntityReplyViewModel>("Home.Topic.Footer", model => new NewEntityReplyViewModel()
                {
                    EntityId = topic.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer")
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic topic, IUpdateModel updater)
        {
            
            var newEntityViewModel = new EditEntityViewModel()
            {
                EntityId = topic.Id
            };

            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => newEntityViewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => newEntityViewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => newEntityViewModel).Zone("Footer")
            ));



        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic viewModel, IUpdateModel updater)
        {

            var topic = await _entityStore.GetByIdAsync(viewModel.Id);
            if (topic == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }
            
            var model = new EditEntityViewModel();
            model.EntityId = viewModel.Id;
            model.Title = viewModel.Title;
      
            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, updater);
            }

            if (updater.ModelState.IsValid)
            {

                var message = string.Empty;
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }


                await _topicManager.UpdateAsync(viewModel);

                if (model.EntityId > 0)
                {
                    var result = await _replyManager.CreateAsync(new EntityReply
                    {
                        EntityId = model.EntityId,
                        Message = message.Trim()
                    });


                    foreach (var error in result.Errors)
                    {
                        updater.ModelState.AddModelError(string.Empty, error.Description);
                    }


                }
                

                //if (result.Succeeded)
                //{

                //}
                //else
                //{
                //    foreach (var error in result.Errors)
                //    {
                //        updater.ModelState.AddModelError(string.Empty, error.Description);
                //    }
                //}


                //if (model.EntityId == 0)
                //{

                //    //var result = await _topicManager.CreateAsync(new Topic()
                //    //{
                //    //    Title = model.Title,
                //    //    Message = message
                //    //});


                //    //foreach (var error in result.Errors)
                //    //{
                //    //    updater.ModelState.AddModelError(string.Empty, error.Description);
                //    //}

                //}
                //else
                //{

                //    var result = await _replyManager.CreateAsync(new EntityReply
                //    {
                //        EntityId = model.EntityId,
                //        Message = message.Trim()
                //    });

                //    foreach (var error in result.Errors)
                //    {
                //        updater.ModelState.AddModelError(string.Empty, error.Description);
                //    }

                //    return await BuildDisplayAsync(viewModel, updater);

                //}

            }

            return await BuildIndexAsync(viewModel, updater);

        }

        #endregion
        
        #region "Private Methods"

        //async Task<HomeIndexViewModel> GetIndexViewModel(
        //    FilterOptions filterOptions,
        //    PagerOptions pagerOptions)
        //{
        //    //var topics = await GetEntities(filterOptions, pagerOptions);
        //    return new HomeIndexViewModel(
        //        topics,
        //        filterOptions,
        //        pagerOptions);
        //}
        
        //async Task<IPagedResults<Topic>> GetEntities(
        //    FilterOptions filterOptions,
        //    PagerOptions pagerOptions)
        //{

        //    // Get current feature (i.e. Plato.Discuss) from area
        //    var feature = await _contextFacade.GetFeatureByAreaAsync();

        //    return await _entityStore.QueryAsync()
        //        .Take(pagerOptions.Page, pagerOptions.PageSize)
        //        .Select<EntityQueryParams>(q =>
        //        {

        //            if (feature != null)
        //            {
        //                q.FeatureId.Equals(feature.Id);
        //            }

        //            q.HideSpam.True();
        //            q.HidePrivate.True();
        //            q.HideDeleted.True();

        //            //q.IsPinned.True();


        //            //if (!string.IsNullOrEmpty(filterOptions.Search))
        //            //{
        //            //    q.UserName.IsIn(filterOptions.Search).Or();
        //            //    q.Email.IsIn(filterOptions.Search);
        //            //}
        //            // q.UserName.IsIn("Admin,Mark").Or();
        //            // q.Email.IsIn("email440@address.com,email420@address.com");
        //            // q.Id.Between(1, 5);
        //        })
        //        .OrderBy("Id", OrderBy.Desc)
        //        .ToList();
        //}
        
        async Task<IPagedResults<EntityReply>> GetEntityReplies(
            int entityId,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _entityReplyStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.Keywords.IsIn(filterOptions.Search);
                    }
                })
                .OrderBy("CreatedDate", OrderBy.Asc)
                .ToList();
        }
        
        int GetPageIndex(IUpdateModel updater)
        {

            var page = 1;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }


        #endregion
        

    }

}
