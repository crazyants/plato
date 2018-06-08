using System;
using System.Threading.Tasks;
using Plato.Layout.Drivers;
using Plato.Layout.ModelBinding;
using Plato.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {
        public override async Task<IViewProviderResult> Display(User user, IUpdateModel updater)
        {

            return Combine(
                await View<User>("User-Display", async model => user),
                await View<User>("User-Display-2", async model => user)
            );
            
            //return await View<UserViewModel>("DisplayUser", model =>
            //{
            //    model.User = user;
            //    return model;
            //});



        }

        public override async Task<IViewProviderResult> Edit(User user, IUpdateModel updater)
        {
            
            return Combine(
                await View<EditUserViewModel>("User-Edit", async model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return model;
                }),
                await View<EditUserViewModel>("User-Edit-2", async model =>
                {
                    model.Id = user.Id.ToString();
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    return model;
                })
            );

        }

        public override async Task<IViewProviderResult> Update(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await Edit(user, updater);
            }

            return await Edit(user, updater);

        }
    }
}
