using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.Drivers;
using Plato.Layout.ModelBinding;
using Plato.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {
        public override async Task<IViewProviderResult> Display(User usersPaged, IUpdateModel updater)
        {

            var user = new User();
            user.UserName = "Ryan Healey 123 (From View Provider)";
            user.Email = "sales@instantasp.co.uk";
            
            return Combine(
                await Initialize<UserViewModel>("DisplayUser", model =>
                {
                    model.User = user;
                    return model;
                }),
                await Initialize<UserViewModel>("DisplayUser2", model =>
                {
                    model.User = user;
                    return model;
                })
            );


            //return await Initialize<UserViewModel>("DisplayUser", model =>
            //{
            //    return model;
            //});



        }

        public override Task<IViewProviderResult> Edit(User usersPaged, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> Update(User usersPaged, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }
}
