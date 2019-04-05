using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Docs.Models;
using Plato.Docs.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Docs.ViewProviders
{

    public class ReplyViewProvider : BaseViewProvider<DocComment>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityReplyStore<DocComment> _replyStore;
        private readonly IPostManager<DocComment> _replyManager;
 
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<TopicViewProvider> stringLocalize,
            IPostManager<DocComment> replyManager, 
            IEntityReplyStore<DocComment> replyStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(DocComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(DocComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(DocComment docComment, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = docComment.Message;
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

            var viewModel = new EditEntityReplyViewModel()
            {
                Id = docComment.Id,
                EntityId = docComment.EntityId,
                EditorHtmlName = EditorHtmlName,
                Message = message
            };

            return Task.FromResult(Views(
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Header", model => viewModel).Zone("header"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Footer", model => viewModel).Zone("Footer")
            ));


        }

        public override async Task<bool> ValidateModelAsync(DocComment docComment, IUpdateModel updater)
        {
            // Build model
            var model = new EditEntityReplyViewModel
            {
                Id = docComment.Id,
                EntityId = docComment.EntityId,
                Message = docComment.Message
            };

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(DocComment docComment, IViewProviderContext context)
        {
            
            if (docComment.IsNewReply)
            {
                return default(IViewProviderResult);
            }

            // Ensure the reply exists
            if (await _replyStore.GetByIdAsync(docComment.Id) == null)
            {
                return await BuildIndexAsync(docComment, context);
            }

            // Validate model
            if (await ValidateModelAsync(docComment, context.Updater))
            {

                // Update reply
                var result = await _replyManager.UpdateAsync(docComment);

                // Was there a problem updating the reply?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(docComment, context);

        }

    }

}
