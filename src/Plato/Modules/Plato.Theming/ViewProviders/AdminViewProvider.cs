using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Theming.Models;
using Plato.Theming.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.ViewProviders
{
    
    public class AdminViewProvider : BaseViewProvider<ThemeAdmin>
    {

        private readonly ISiteThemeManager _siteThemeManager;
        private readonly ISiteThemeFileManager _siteThemeFileManager;
        private readonly IThemeManager _themeManager;
        
        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public AdminViewProvider(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer, 
            IThemeManager themeManager, 
            ISiteThemeManager siteThemeManager,
            ISiteThemeFileManager siteThemeFileManager)
        {
            _themeManager = themeManager;
            _siteThemeManager = siteThemeManager;
            _siteThemeFileManager = siteThemeFileManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        public override Task<IViewProviderResult> BuildDisplayAsync(ThemeAdmin model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            var indexViewModel = new ThemingIndexViewModel()
            {
                Themes = _siteThemeManager.AvailableThemes
            };

            return Task.FromResult(Views(
                View<ThemingIndexViewModel>("Admin.Index.Header", viewModel => indexViewModel).Zone("header"),
                View<ThemingIndexViewModel>("Admin.Index.Tools", viewModel => indexViewModel).Zone("tools"),
                View<ThemingIndexViewModel>("Admin.Index.Content", viewModel => indexViewModel).Zone("content")
            ));
            
        }

        public override async Task<IViewProviderResult> BuildEditAsync(ThemeAdmin model, IViewProviderContext updater)
        {

            // We are adding a new theme
            if (String.IsNullOrEmpty(model.ThemeId))
            {
                var createViewModel = new CreateThemeViewModel()
                {
                    AvailableThemes = GetAvailableThemes()
                };

                return Views(
                    View<CreateThemeViewModel>("Admin.Create.Header", viewModel => createViewModel).Zone("header").Order(1),
                    View<CreateThemeViewModel>("Admin.Create.Content", viewModel => createViewModel).Zone("content").Order(1),
                    View<CreateThemeViewModel>("Admin.Create.Footer", viewModel => createViewModel).Zone("footer").Order(1)
                );
            }

            var file = _siteThemeFileManager.GetFile(model.ThemeId, model.Path);

            // We are editing an existing theme
            var editViewModel = new EditThemeViewModel()
            {
                ThemeId = model.ThemeId,
                Path = model.Path,
                File = file,
                FileContents = await _siteThemeFileManager.ReadFileAsync(file),
                Files = !string.IsNullOrEmpty(model.Path)
                    ? _siteThemeFileManager.GetFiles(model.ThemeId, model.Path)
                    : _siteThemeFileManager.GetFiles(model.ThemeId)
            };

            return Views(
                View<EditThemeViewModel>("Admin.Edit.Header", viewModel => editViewModel).Zone("header").Order(1),
                View<EditThemeViewModel>("Admin.Edit.Content", viewModel => editViewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ThemeAdmin theme, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }


        IEnumerable<SelectListItem> GetAvailableThemes()
        {

            var themes = new List<SelectListItem>();
            foreach (var theme in _themeManager.AvailableThemes)
            {
                themes.Add(new SelectListItem
                {
                    Text = theme.Name,
                    Value = theme.Id
                });
            }

            return themes;
        }


    }

}
