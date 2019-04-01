using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Theming.Models;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Theming.ViewModels;

namespace Plato.Theming.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<ThemeAdmin> _viewProvider;
        private readonly ISiteThemeCreator _siteThemeCreator;
        private readonly ISiteThemeManager _siteThemeManager;
        private readonly ISiteThemeFileManager _themeFileManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,           
            IViewProviderManager<ThemeAdmin> viewProvider,
            ISiteThemeCreator siteThemeCreator,
            IBreadCrumbManager breadCrumbManager,
            IContextFacade contextFacade,
            IAlerter alerter,
            ISiteThemeFileManager themeFileManager,
            ISiteThemeManager siteThemeManager)
        {

            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _alerter = alerter;
            _themeFileManager = themeFileManager;
            _siteThemeManager = siteThemeManager;
            _siteThemeCreator = siteThemeCreator;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        // ------------
        // Index
        // ------------

        public async Task<IActionResult> Index()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Themes"]);
            });
                     
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new ThemeAdmin(), this));
            
        }
        
        // ------------
        // Create
        // ------------

        public async Task<IActionResult> Create()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Theming"], tags => tags
                        .Action("Index", "Admin", "Plato.Theming")
                        .LocalNav())
                    .Add(S["Add Theme"]);
            });

            // We need to pass along the featureId
            return View((LayoutViewModel)await _viewProvider.ProvideEditAsync(new ThemeAdmin(), this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Create))]
        public async Task<IActionResult> CreatePost(CreateThemeViewModel viewModel)
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {

                // Add model state errors 
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

                // Return
                return RedirectToAction(nameof(Index));

            }

            // Create theme
            var model = new ThemeAdmin();

            var result = _siteThemeCreator.CreateTheme(viewModel.ThemeId, viewModel.Name);
            if (result.Succeeded)
            {

                // Execute view providers
               await _viewProvider.ProvideUpdateAsync(model, this);

                // Add confirmation
                _alerter.Success(T["Theme Added Successfully!"]);

                // Return
                return RedirectToAction(nameof(Index));

            }
            else
            {
                // Report any errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(viewModel);

        }

        // ------------
        // Edit
        // ------------

        public async Task<IActionResult> Edit(string id, string path)
        {

            var theme = _siteThemeManager
                .AvailableThemes.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (theme == null)
            {
                return NotFound();
            }
            
            // Get theme files for current theme and path
            var themeFile = _themeFileManager.GetFile(id, path);

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Themes"], theming => theming
                        .Action("Index", "Admin", "Plato.Theming")
                        .LocalNav());
             
                // Build parents
                var parents = _themeFileManager.GetParents(id, path);
                if (parents != null)
                {

                    builder.Add(S[theme.Name], home => home
                        .Action("Edit", "Admin", "Plato.Theming", new RouteValueDictionary()
                        {
                            ["id"] = theme.Id,
                            ["path"] = ""
                        }).LocalNav());

                    foreach (var parent in parents)
                    {

                        if (string.IsNullOrEmpty(parent.RelativePath))
                        {
                            // don't render root - handled above
                            continue;
                        }

                        if (parent.RelativePath.Equals(path, StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Add(S[parent.Name], home => home 
                                .LocalNav());
                        }
                        else
                        {
                            builder.Add(S[parent.Name], home => home
                                .Action("Edit", "Admin", "Plato.Theming", new RouteValueDictionary()
                                {
                                    ["id"] = theme.Id,
                                    ["path"] = parent.RelativePath
                                }).LocalNav());
                        }
               
                    }
                }
                else
                {
                    builder.Add(S[theme.Name], home => home
                        .LocalNav());
                }

            });
            
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new ThemeAdmin()
            {
                ThemeId = id,
                Path = path
            }, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditThemeViewModel model)
        {
            
            var result = await _viewProvider.ProvideUpdateAsync(new ThemeAdmin(), this);

            if (!ModelState.IsValid)
            {

                // Add model state errors 
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }

                // Return
                return RedirectToAction(nameof(Index));

            }

            // Get theme files for current theme and path
            var themeFile = _themeFileManager.GetFile(model.ThemeId, model.Path);

            if (themeFile == null)
            {
                return NotFound();
            }

            // Write file
            await _themeFileManager.SaveFileAsync(themeFile, model.FileContents);

            // Display Confirmation
            _alerter.Success(T["File Updated Successfully!"]);

            // Redirect back to topic
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Theming",
                ["controller"] = "Admin",
                ["action"] = "Edit",
                ["id"] = model.ThemeId,
                ["path"] = model.Path
            }));

        }
        
    }

}
