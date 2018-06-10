using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;
using Plato.Layout.ViewProviders;
using Plato.Models.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserListViewProvider : BaseViewProvider<UsersPagedViewModel>
    {

        public override async Task<IViewProviderResult> BuildDisplayAsync(UsersPagedViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(UsersPagedViewModel viewModel, IUpdateModel updater)
        {

            return Views(
                View<UsersPagedViewModel>("User.List", model => viewModel).Zone("content").Order(1)
            );


        }

        public override async Task<IViewProviderResult> BuildEditAsync(UsersPagedViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(UsersPagedViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }
}
