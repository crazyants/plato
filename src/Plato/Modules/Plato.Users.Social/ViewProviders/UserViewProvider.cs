using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Social.ViewModels;

namespace Plato.Users.Social.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IUrlHelper _urlHelper;

        public UserViewProvider(
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IHostingEnvironment hostEnvironment,
            IUrlHelperFactory urlHelperFactory,
            IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _userPhotoStore = userPhotoStore;
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
        }


        public override async Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {
            return null;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return null;
        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {
        
            return Views(
                View<EditSocialViewModel>("Social.Edit.Content", model =>
                {
                    model.FacebookUrl = "";
                    return model;
                }).Order(10)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditSocialViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }

            //model.FacebookUrl = model.UserName?.Trim();

            if (updater.ModelState.IsValid)
            {
                
                var result = await _userManager.UpdateAsync(user);

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            
            return await BuildEditAsync(user, updater);

        }
        
    }
}
