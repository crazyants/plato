using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Theming.Models;
using Plato.Theming.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Theming.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<ThemeAdmin>
    {

        public override Task<IViewProviderResult> BuildDisplayAsync(ThemeAdmin model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            var indexViewModel = new ThemingIndexViewModel();

            return Task.FromResult(Views(
                View<ThemingIndexViewModel>("Admin.Index.Header", viewModel => indexViewModel).Zone("header"),
                View<ThemingIndexViewModel>("Admin.Index.Content", viewModel => indexViewModel).Zone("content")
            ));


        }

        public override Task<IViewProviderResult> BuildEditAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            var editThemeViewModel = new EditThemeViewModel();

            return Task.FromResult(Views(
                View<EditThemeViewModel>("Admin.Edit.Header", viewModel => editThemeViewModel).Zone("header").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Content", viewModel => editThemeViewModel).Zone("content").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Actions", viewModel => editThemeViewModel).Zone("actions").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Footer", viewModel => editThemeViewModel).Zone("footer").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ThemeAdmin model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}
