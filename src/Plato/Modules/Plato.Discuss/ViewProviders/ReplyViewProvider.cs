using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.ViewProviders
{

    public class ReplyViewProvider : BaseViewProvider<Reply>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityReplyStore<Reply> _replyStore;
        private readonly IPostManager<Reply> _replyManager;
 
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<TopicViewProvider> stringLocalize,
            IPostManager<Reply> replyManager, 
            IEntityReplyStore<Reply> replyStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }



        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Reply reply, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = reply.Message;
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

            var viewModel = new EditReplyViewModel()
            {
                Id = reply.Id,
                EntityId = reply.EntityId,
                EditorHtmlName = EditorHtmlName,
                Message = message
            };

            return Task.FromResult(Views(
                View<EditReplyViewModel>("Home.Edit.Reply.Header", model => viewModel).Zone("header"),
                View<EditReplyViewModel>("Home.Edit.Reply.Content", model => viewModel).Zone("content"),
                View<EditReplyViewModel>("Home.Edit.Reply.Footer", model => viewModel).Zone("Footer")
            ));


        }

        public override async Task<bool> ValidateModelAsync(Reply reply, IUpdateModel updater)
        {
            // Build model
            var model = new EditReplyViewModel();
            model.Id = reply.Id;
            model.EntityId = reply.EntityId;
            model.Message = reply.Message;

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {
            
            if (reply.IsNewReply)
            {
                return default(IViewProviderResult);
            }

            // Ensure the reply exists
            if (await _replyStore.GetByIdAsync(reply.Id) == null)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Validate model
            if (await ValidateModelAsync(reply, context.Updater))
            {

                // Update reply
                var result = await _replyManager.UpdateAsync(reply);

                // Was there a problem updating the reply?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(reply, context);

        }

    }

}
