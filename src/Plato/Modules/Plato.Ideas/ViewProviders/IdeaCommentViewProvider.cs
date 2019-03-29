using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Ideas.Services;
using Plato.Ideas.ViewModels;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Ideas.ViewProviders
{

    public class IdeaCommentViewProvider : BaseViewProvider<IdeaComment>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityReplyStore<IdeaComment> _replyStore;
        private readonly IPostManager<IdeaComment> _replyManager;
 
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public IdeaCommentViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<IdeaViewProvider> stringLocalize,
            IPostManager<IdeaComment> replyManager, 
            IEntityReplyStore<IdeaComment> replyStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }
        

        public override Task<IViewProviderResult> BuildDisplayAsync(IdeaComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(IdeaComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(IdeaComment ideaComment, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = ideaComment.Message;
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
                Id = ideaComment.Id,
                EntityId = ideaComment.EntityId,
                EditorHtmlName = EditorHtmlName,
                Message = message
            };

            return Task.FromResult(Views(
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Header", model => viewModel).Zone("header"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Footer", model => viewModel).Zone("Footer")
            ));

        }

        public override async Task<bool> ValidateModelAsync(IdeaComment ideaComment, IUpdateModel updater)
        {
            // Build model
            var model = new EditEntityReplyViewModel
            {
                Id = ideaComment.Id,
                EntityId = ideaComment.EntityId,
                Message = ideaComment.Message
            };

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(IdeaComment ideaComment, IViewProviderContext context)
        {
            
            if (ideaComment.IsNewAnswer)
            {
                return default(IViewProviderResult);
            }

            // Ensure the reply exists
            if (await _replyStore.GetByIdAsync(ideaComment.Id) == null)
            {
                return await BuildIndexAsync(ideaComment, context);
            }

            // Validate model
            if (await ValidateModelAsync(ideaComment, context.Updater))
            {

                // Update reply
                var result = await _replyManager.UpdateAsync(ideaComment);

                // Was there a problem updating the reply?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(ideaComment, context);

        }

    }

}
