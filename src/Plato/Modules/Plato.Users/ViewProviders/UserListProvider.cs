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
    public class UserListProvider : BaseViewProvider<User>
    {
        public override async Task<IViewResult> BuildDisplayAsync(User usersPaged, IUpdateModel updater)
        {

            return await Initialize<UserViewModel>("UserList2", model =>
            {
                return model;
            });

        }

        public override Task<IViewResult> BuildEditAsync(User usersPaged, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewResult> BuildUpdateAsync(User usersPaged, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }
}
