using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.ViewProviders
{
    public class HomeTopicViewProvider : BaseViewProvider<HomeTopicViewModel>
    {

        private const string EditorHtmlName = "message";


        private readonly IEntityManager<EntityReply> _entityReplyManager;

        private readonly HttpRequest _request;


        public HomeTopicViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityManager<EntityReply> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeTopicViewModel viewMode, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(HomeTopicViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(
                Views(
                    View<HomeTopicViewModel>("Home.Topic.Header", model =>
                    {
                        viewModel.EditorHtmlName= EditorHtmlName;
                        return viewModel;
                    }).Zone("header"),
                    View<HomeTopicViewModel>("Home.Topic.Tools", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("tools"),
                    View<HomeTopicViewModel>("Home.Topic.Sidebar", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("sidebar"),
                    View<HomeTopicViewModel>("Home.Topic.Content", model =>
                    {
                        viewModel.EditorHtmlName = EditorHtmlName;
                        return viewModel;
                    }).Zone("content")
                ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeTopicViewModel viewMode, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(HomeTopicViewModel viewModel, IUpdateModel updater)
        {

            var model = new HomeTopicViewModel();
        
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

                var reply = new EntityReply();
                reply.EntityId = viewModel.Entity.Id;
                reply.Message = message.Trim();
                
                var result = await _entityReplyManager.CreateAsync(reply);

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildIndexAsync(viewModel, updater);
            
        }
        

    }

}
