using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class AdminIndexViewProvider : BaseViewProvider<RolesIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(RolesIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        public override  Task<IViewProviderResult> BuildIndexAsync(RolesIndexViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(
                Views(
                    View<RolesIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                    View<RolesIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                    View<RolesIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
                ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(RolesIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(RolesIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }
}
