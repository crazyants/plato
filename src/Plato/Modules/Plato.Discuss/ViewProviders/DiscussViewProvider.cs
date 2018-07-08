using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    public class DiscussViewProvider : BaseViewProvider<Entity>
    {

        private const string EditorHtmlName = "message";

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IPostManager<EntityReply> _replyManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly HttpRequest _request;
        
        public DiscussViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IPostManager<EntityReply> replyManager,
            IEntityReplyStore<EntityReply> entityReplyStore,
            IActionContextAccessor actionContextAccessor,
            IContextFacade contextFacade, IEntityStore<Entity> entityStore)
        {
            _replyManager = replyManager;
            _entityReplyStore = entityReplyStore;
            _actionContextAccessor = actionContextAccessor;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Entity entity, IUpdateModel updater)
        {
           
            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = GetPageIndex();

            var replies = await GetEntityReplies(entity.Id, filterOptions, pagerOptions);

            var topivViewModel = new HomeTopicViewModel(replies, pagerOptions)
            {
                Entity = entity,
                EditorHtmlName = EditorHtmlName,
            };

            return Views(
                View<HomeTopicViewModel>("Home.Topic.Header", model => topivViewModel).Zone("header"),
                View<HomeTopicViewModel>("Home.Topic.Tools", model => topivViewModel).Zone("tools"),
                View<HomeTopicViewModel>("Home.Topic.Sidebar", model => topivViewModel).Zone("sidebar"),
                View<HomeTopicViewModel>("Home.Topic.Content", model => topivViewModel).Zone("content")
            );

        }


        public override async Task<IViewProviderResult> BuildIndexAsync(Entity entity, IUpdateModel updater)
        {

            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = GetPageIndex();

            var indexViewModel = await GetIndexViewModel(filterOptions, pagerOptions);
            indexViewModel.EditorHtmlName = EditorHtmlName;
            
            return Views(
                View<HomeIndexViewModel>("Home.Index.Header", model => indexViewModel).Zone("header"),
                View<HomeIndexViewModel>("Home.Index.Tools", model => indexViewModel).Zone("tools"),
                View<HomeIndexViewModel>("Home.Index.Sidebar", model => indexViewModel).Zone("sidebar"),
                View<HomeIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content")
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Entity entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Entity entity, IUpdateModel updater)
        {

            var model = new HomeTopicViewModel();
            
            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(entity, updater);
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

                var reply = new EntityReply();
                reply.EntityId = entity.Id;
                reply.Message = message.Trim();
                
                var result = await _replyManager.CreateAsync(reply);

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildIndexAsync(entity, updater);
            
        }




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
                    if (!string.IsNullOrEmpty(filterOptions.Search))
                    {
                        q.Keywords.IsIn(filterOptions.Search);
                    }
                })
                .OrderBy("CreatedDate", OrderBy.Asc)
                .ToList();
        }


        private int GetPageIndex()
        {

            var page = 1;
            var routeData = _actionContextAccessor.ActionContext.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }



    }

}
