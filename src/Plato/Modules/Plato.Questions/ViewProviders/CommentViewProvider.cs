using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Questions.Models;
using Plato.Questions.Services;
using Plato.Questions.ViewModels;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Questions.ViewProviders
{

    public class CommentViewProvider : BaseViewProvider<Answer>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityReplyStore<Answer> _replyStore;
        private readonly IPostManager<Answer> _replyManager;
 
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public CommentViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<ArticleViewProvider> stringLocalize,
            IPostManager<Answer> replyManager, 
            IEntityReplyStore<Answer> replyStore)
        {

            _replyManager = replyManager;
            _replyStore = replyStore;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }
        

        public override Task<IViewProviderResult> BuildDisplayAsync(Answer model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Answer model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Answer answer, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = answer.Message;
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
                Id = answer.Id,
                EntityId = answer.EntityId,
                EditorHtmlName = EditorHtmlName,
                Message = message
            };

            return Task.FromResult(Views(
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Header", model => viewModel).Zone("header"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Edit.Reply.Footer", model => viewModel).Zone("Footer")
            ));


        }

        public override async Task<bool> ValidateModelAsync(Answer answer, IUpdateModel updater)
        {
            // Build model
            var model = new EditEntityReplyViewModel
            {
                Id = answer.Id,
                EntityId = answer.EntityId,
                Message = answer.Message
            };

            // Validate model
            return await updater.TryUpdateModelAsync(model);

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Answer answer, IViewProviderContext context)
        {
            
            if (answer.IsNewAnswer)
            {
                return default(IViewProviderResult);
            }

            // Ensure the reply exists
            if (await _replyStore.GetByIdAsync(answer.Id) == null)
            {
                return await BuildIndexAsync(answer, context);
            }

            // Validate model
            if (await ValidateModelAsync(answer, context.Updater))
            {

                // Update reply
                var result = await _replyManager.UpdateAsync(answer);

                // Was there a problem updating the reply?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(answer, context);

        }

    }

}
