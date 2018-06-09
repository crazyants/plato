using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Layout.ViewProviders;
using Plato.Layout.ModelBinding;
using Plato.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;

        public UserViewProvider(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        public override async Task<IViewProviderResult> DisplayAsync(User user, IUpdateModel updater)
        {

            return Views(
                await View<User>("User.Display", model => Task.FromResult(user)),
                await View<User>("User.Display", model => Task.FromResult(user)),
                await View<User>("User.Display-2", model => Task.FromResult(user))
            );

            //return await View<UserViewModel>("DisplayUser", model =>
            //{
            //    model.User = user;
            //    return model;
            //});
            
        }

        public override async Task<IViewProviderResult> EditAsync(User user, IUpdateModel updater)
        {
            
            return Views(
                await View<User>("User.Display", model => Task.FromResult(user)),
                await View<User>("User.Display", model => Task.FromResult(user)),
                await View<User>("User.Display-2", model => Task.FromResult(user)),
                await View<EditUserViewModel>("User.Edit", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return Task.FromResult(model);
                }),
                await View<EditUserViewModel>("User.Edit-2", model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return Task.FromResult(model);
                })
            );

        }

        public override async Task<IViewProviderResult> UpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await EditAsync(user, updater);
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
            
            return await EditAsync(user, updater);

        }
    }
}
