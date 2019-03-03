using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditSignatureViewProvider : BaseViewProvider<EditSignatureViewModel>
    {
        
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IPlatoUserManager<User> _platoUserManager;
 
        private readonly HttpRequest _request;

        public EditSignatureViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore,
            IPlatoUserManager<User> platoUserManager)
        {
            _platoUserStore = platoUserStore;
            _platoUserManager = platoUserManager;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditSignatureViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditSignatureViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditSignatureViewModel viewModel, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var signature = viewModel.Signature;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == viewModel.EditorHtmlName)
                    {
                        signature = _request.Form[key];
                    }
                }
            }
            
            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }

            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("sidebar"),
                View<User>("Home.Edit.Tools", model => user).Zone("tools"),
                View<EditSignatureViewModel>("Home.EditSignature.Content", model => viewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditSignatureViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(viewModel);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditSignatureViewModel viewModel, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }

            var model = new EditSignatureViewModel();;

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                user.Signature = model.Signature;
             
                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(viewModel, context);

        }

        #endregion

      
    }

}
