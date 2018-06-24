using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserIndexViewProvider : BaseViewProvider<UsersIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(UsersIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        public override Task<IViewProviderResult> BuildIndexAsync(UsersIndexViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(
                Views(
                    View<UsersIndexViewModel>("User.Index.Header", model => viewModel).Zone("header"),
                    View<UsersIndexViewModel>("User.Index.Tools", model => viewModel).Zone("tools"),
                    View<UsersIndexViewModel>("User.Index.Content", model => viewModel).Zone("content")
                ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UsersIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UsersIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
