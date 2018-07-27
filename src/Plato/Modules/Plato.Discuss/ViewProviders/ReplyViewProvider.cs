using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.ViewProviders
{

    public class ReplyViewProvider : BaseViewProvider<Reply>
    {

        private const string LabelHtmlName = "message";
        
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IContextFacade contextFacade,
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<TopicViewProvider> stringLocalize)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _request = httpContextAccessor.HttpContext.Request;
            T = stringLocalize;
        }



        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Reply model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(Reply reply, IUpdateModel updater)
        {
            // Build model
            var model = new EditReplyViewModel();
            model.EntityId = reply.EntityId;
            model.Message = reply.Message;

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Reply model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }
}
