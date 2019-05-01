using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Issues.Models;
using Plato.Issues.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Issues.ViewProviders
{

    public class CommentViewProvider : BaseViewProvider<Comment>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityReplyStore<Comment> _replyStore;
        private readonly IPostManager<Comment> _replyManager;
 
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public CommentViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<IssueViewProvider> stringLocalize,
            IPostManager<Comment> replyManager, 
            IEntityReplyStore<Comment> replyStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }
        

        public override Task<IViewProviderResult> BuildDisplayAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Comment comment, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = comment.Message;
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
                Id = comment.Id,
                EntityId = comment.EntityId,
                EditorHtmlName = EditorHtmlName,
                Message = message
            };

            return Task.FromResult(Views(
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Header", model => viewModel).Zone("header"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Footer", model => viewModel).Zone("Footer")
            ));


        }

        public override async Task<bool> ValidateModelAsync(Comment comment, IUpdateModel updater)
        {
            // Build model
            var model = new EditEntityReplyViewModel
            {
                Id = comment.Id,
                EntityId = comment.EntityId,
                Message = comment.Message
            };

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Comment comment, IViewProviderContext context)
        {
            
            if (comment.IsNewReply)
            {
                return default(IViewProviderResult);
            }

            // Ensure the reply exists
            if (await _replyStore.GetByIdAsync(comment.Id) == null)
            {
                return await BuildIndexAsync(comment, context);
            }

            // Validate model
            if (await ValidateModelAsync(comment, context.Updater))
            {

                // Update reply
                var result = await _replyManager.UpdateAsync(comment);

                // Was there a problem updating the reply?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(comment, context);

        }

    }

}
