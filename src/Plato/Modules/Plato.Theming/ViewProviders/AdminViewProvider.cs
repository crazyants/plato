using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Theming.Models;
using Plato.Theming.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Theming.Abstractions;
using Plato.Theming.Services;

namespace Plato.Theming.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<ThemeAdmin>
    {

        private readonly IThemeManager _themeManager;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer, 
            IThemeManager themeManager)
        {
            _themeManager = themeManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        public override Task<IViewProviderResult> BuildDisplayAsync(ThemeAdmin model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            var indexViewModel = new ThemingIndexViewModel();

            return Task.FromResult(Views(
                View<ThemingIndexViewModel>("Admin.Index.Header", viewModel => indexViewModel).Zone("header"),
                View<ThemingIndexViewModel>("Admin.Index.Tools", viewModel => indexViewModel).Zone("tools"),
                View<ThemingIndexViewModel>("Admin.Index.Content", viewModel => indexViewModel).Zone("content")
            ));
            
        }

        public override Task<IViewProviderResult> BuildEditAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            var editThemeViewModel = new EditThemeViewModel()
            {
                IsNewTheme = model.IsNewTheme,
                AvailableThemes = GetAvailableThemes()
            };

            return Task.FromResult(Views(
                View<EditThemeViewModel>("Admin.Edit.Header", viewModel => editThemeViewModel).Zone("header").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Content", viewModel => editThemeViewModel).Zone("content").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Actions", viewModel => editThemeViewModel).Zone("actions").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Footer", viewModel => editThemeViewModel).Zone("footer").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ThemeAdmin theme, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        IEnumerable<SelectListItem> GetAvailableThemes()
        {

            var themes = new List<SelectListItem>();
            foreach (var z in _themeManager.AvailableThemes)
            {
                themes.Add(new SelectListItem
                {
                    Text = z.Name,
                    Value = z.Id
                });
            }

            return themes;
        }


    }

}
