using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewProviders
{
    public class EntityViewProvider : BaseViewProvider<Entity>
    {

        private const string EditorHtmlName = "message";

        private readonly IEntityReplyStore<EntityReply> _entityReplyStore;
        private readonly IPostManager<EntityReply> _replyManager;
        private readonly HttpRequest _request;
        
        public EntityViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IPostManager<EntityReply> replyManager,
            IEntityReplyStore<EntityReply> entityReplyStore)
        {
            _replyManager = replyManager;
            _entityReplyStore = entityReplyStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Entity entity, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Entity entity, IUpdateModel updater)
        {
            var filterOptions = new FilterOptions();
            var pagerOptions = new PagerOptions();

            var replies = await GetEntityReplies(entity.Id, filterOptions, pagerOptions);

            var topivViewModel = new HomeTopicViewModel()
            {
                Entity = entity,
                Results = replies,
                EditorHtmlName = EditorHtmlName,
            };

            return Views(
                View<HomeTopicViewModel>("Home.Topic.Header", model => topivViewModel).Zone("header"),
                View<HomeTopicViewModel>("Home.Topic.Tools", model => topivViewModel).Zone("tools"),
                View<HomeTopicViewModel>("Home.Topic.Sidebar", model => topivViewModel).Zone("sidebar"),
                View<HomeTopicViewModel>("Home.Topic.Content", model => topivViewModel).Zone("content")
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






    }

}
