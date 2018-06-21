using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUrlHelper _urlHelper;

        public UserViewProvider(
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IHostingEnvironment hostEnvironment,
            IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
        }


        public override async Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {

            return Views(
                View<User>("User.Display.Header", model => user).Zone("header"),
                View<User>("User.Display.Meta", model => user).Zone("meta"),
                View<User>("User.Display.Content", model => user).Zone("content"),
                View<User>("User.Display.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return Views(
                View<User>("User.List", model => user).Zone("header").Order(3)
            );

        }

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {
            
            var photoUrl = _urlHelper.RouteUrl(new UrlRouteContext
            {
                Values = new RouteValueDictionary()
                {
                    {"Area", "Plato.Users"},
                    {"Controller", "Photo"},
                    {"Action", "Upload"}
                }
            });
            
            return Views(
                View<User>("User.Edit.Header", model => user).Zone("header"),
                View<User>("User.Edit.Meta", model => user).Zone("meta"),
                View<EditUserViewModel>("User.Edit.Content", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.PhotoUrl = photoUrl;
                    return model;
                }).Zone("content"),
                View<EditUserViewModel>("User.Edit.Footer", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.PhotoUrl = photoUrl;
                    return model;
                }).Zone("footer"),
                View<EditUserViewModel>("User.Edit.Actions", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.PhotoUrl = photoUrl;
                    return model;
                }).Zone("actions")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }

            model.UserName = model.UserName?.Trim();
            model.Email = model.Email?.Trim();

            if (updater.ModelState.IsValid)
            {

                await _userManager.SetUserNameAsync(user, model.UserName);
                await _userManager.SetEmailAsync(user, model.Email);

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
