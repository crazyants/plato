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

            return await Initialize<UserViewModel>("DisplayUser", model =>
            {
                return model;
            });

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
