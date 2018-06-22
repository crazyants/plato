using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class RoleIndexViewProvider : BaseViewProvider<RolesIndexViewModel>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(RolesIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        public override async Task<IViewProviderResult> BuildIndexAsync(RolesIndexViewModel viewModel, IUpdateModel updater)
        {
            return Views(
                View<RolesIndexViewModel>("Role.Index.Header", model => viewModel).Zone("header"),
                View<RolesIndexViewModel>("Role.Index.Tools", model => viewModel).Zone("tools"),
                View<RolesIndexViewModel>("Role.Index.Content", model => viewModel).Zone("content")
            );
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
