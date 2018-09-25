using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Discuss.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string EditorHtmlName = "message";

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        private readonly IPostManager<Topic> _topicManager;
        private readonly IPostManager<Reply> _replyManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly HttpRequest _request;
        
        public TopicViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IPostManager<Reply> replyManager,
            IEntityReplyStore<Reply> entityReplyStore,
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

        public override Task<IViewProviderResult> BuildIndexAsync(Topic topic, IUpdateModel updater)
        {

            // Build view model
            var viewModel = new TopicIndexViewModel
            {
                Options = new TopicIndexOptions(updater.RouteData),
                Pager = new PagerOptions(updater.RouteData)
            };

            return Task.FromResult(Views(
                View<TopicIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<TopicIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<TopicIndexViewModel>("Home.Index.Sidebar", model => viewModel).Zone("sidebar").Order(3),
                View<TopicIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic viewModel, IUpdateModel updater)
        {

            // Get view data
            var viewOptions = new TopicIndexOptions(updater.RouteData);
            var pagerOptions = new PagerOptions(updater.RouteData);

            // Get entity
            var topic = await _entityStore.GetByIdAsync(viewModel.Id);
            if (topic == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }

            // Increment entity view count
            await IncrementTopicViewCount(topic);

            // Build view model
            var replies = await GetEntityReplies(topic.Id, viewOptions, pagerOptions);
            
            var topivViewModel = new HomeTopicViewModel(replies, pagerOptions)
            {
                Entity = topic
            };
            
            return Views(
                View<HomeTopicViewModel>("Home.Topic.Header", model => topivViewModel).Zone("header"),
                View<HomeTopicViewModel>("Home.Topic.Tools", model => topivViewModel).Zone("tools"),
                View<HomeTopicViewModel>("Home.Topic.Sidebar", model => topivViewModel).Zone("sidebar"),
                View<HomeTopicViewModel>("Home.Topic.Content", model => topivViewModel).Zone("content"),
                View<EditReplyViewModel>("Home.Topic.Footer", model => new EditReplyViewModel()
                {
                    EntityId = topic.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer")
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic topic, IUpdateModel updater)
        {

            // Ensures we persist the message between post backs
            var message = topic.Message;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }
            }
          
            var viewModel = new EditTopicViewModel()
            {
                Id = topic.Id,
                Title = topic.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = topic.Alias
            };
     
            return Task.FromResult(Views(
                View<EditTopicViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditTopicViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditTopicViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditTopicViewModel
            {
                Title = topic.Title,
                Message = topic.Message
            });
        }

        public override async Task ComposeTypeAsync(Topic topic, IUpdateModel updater)
        {

            var model = new EditTopicViewModel
            {
                Title = topic.Title,
                Message = topic.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                topic.Title = model.Title;
                topic.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IUpdateModel updater)
        {

            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, updater);
            }
            
            // Validate 
            if (await ValidateModelAsync(topic, updater))
            {
                
                // Update
                var result = await _topicManager.UpdateAsync(topic);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(topic, updater);

        }

        #endregion
        
        #region "Private Methods"
        
        async Task<IPagedResults<Reply>> GetEntityReplies(
            int entityId,
            TopicIndexOptions topicIndexOptions,
            PagerOptions pagerOptions)
        {
            return await _entityReplyStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(entityId);
                    if (!string.IsNullOrEmpty(topicIndexOptions.Search))
                    {
                        q.Keywords.IsIn(topicIndexOptions.Search);
                    }
                    q.IsSpam.False();
                    q.IsPrivate.False();
                    q.IsDeleted.False();
                })
                .OrderBy("CreatedDate", OrderBy.Asc)
                .ToList();
        }
        
        //TopicIndexOptions GetViewOptions(IUpdateModel updater)
        //{
        //    if (updater.ViewData["opts"] is TopicIndexOptions opts)
        //    {
        //        return opts;
        //    }
          
        //    return new TopicIndexOptions();
        //}

        //PagerOptions GetPagerOptions(IUpdateModel updater)
        //{
        //    if (updater.ViewData["pager"] is PagerOptions pager)
        //    {
        //        return pager;
        //    }

        //    return new PagerOptions();
        //}


        //string GetKeywords(IUpdateModel updater)
        //{

        //    var keywords = string.Empty;
        //    var routeData = updater.RouteData;
        //    var found = routeData.Values.TryGetValue("search", out object value);
        //    if (found && value != null)
        //    {
        //        keywords = value.ToString();
        //    }

        //    return keywords;

        //}

        async Task IncrementTopicViewCount(Topic topic)
        {
            
            topic.TotalViews = topic.TotalViews + 1;
            topic.DailyViews = topic.TotalViews.ToSafeDevision(DateTimeOffset.Now.DayDifference(topic.CreatedDate));

            await _entityStore.UpdateAsync(topic);

        }

        #endregion

    }

}
