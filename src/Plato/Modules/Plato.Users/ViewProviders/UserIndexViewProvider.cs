using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserIndexViewProvider : BaseViewProvider<UsersPagedViewModel>
    {

        public override async Task<IViewProviderResult> BuildDisplayAsync(UsersPagedViewModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(UsersPagedViewModel viewModel, IUpdateModel updater)
        {

            return Views(
                View<UsersPagedViewModel>("User.Index.Header", model => viewModel).Zone("header"),
                View<UsersPagedViewModel>("User.Index.Tools", model => viewModel).Zone("tools"),
                View<UsersPagedViewModel>("User.Index.Content", model => viewModel).Zone("content")
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
