using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Theming.Abstractions;
using Plato.Settings.ViewModels;

namespace Plato.Settings.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ISiteThemeLoader _themeLoader;
        private readonly ILocaleProvider _localeProvider;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IAuthorizationService authorizationService,
            ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            ITimeZoneProvider timeZoneProvider,
            ILocaleProvider localeProvider,
            ISiteThemeLoader themeLoader,
            IPlatoHost platoHost,
            IShellSettings shellSettings,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _timeZoneProvider = timeZoneProvider;
            _localeProvider = localeProvider;
            _themeLoader = themeLoader;
            _alerter = alerter;
            _platoHost = platoHost;
            _shellSettings = shellSettings;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["General"]);
            });
            
            return View(await GetModel());

        }

        public async Task<IActionResult> CreateApiKey()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Key Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SiteSettingsViewModel viewModel)
        {
            
            // TODO: Implement security
            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }
                return View(await GetModel());
            }

            // Update existing settings
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                settings.SiteName = viewModel.SiteName;
                settings.TimeZone = viewModel.TimeZone;
                settings.DateTimeFormat = viewModel.DateTimeFormat;
                settings.Culture = viewModel.Culture;
                settings.Theme = viewModel.Theme;
            }
            else
            {
                // Create new settings
                settings = new SiteSettings()
                {
                    SiteName = viewModel.SiteName,
                    TimeZone = viewModel.TimeZone,
                    DateTimeFormat = viewModel.DateTimeFormat,
                    Culture = viewModel.Culture,
                    Theme = viewModel.Theme
                };
            }
        
            // Update settings
            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {

                // Recycle shell context to ensure changes take effect
                _platoHost.RecycleShellContext(_shellSettings);

                _alerter.Success(T["Settings Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
            
        }
        
        #endregion

        #region "Private Methods"

        private async Task<SiteSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                return new SiteSettingsViewModel()
                {
                    SiteName = settings.SiteName,
                    TimeZone = settings.TimeZone,
                    DateTimeFormat = settings.DateTimeFormat,
                    Culture = settings.Culture,
                    Theme = settings.Theme,
                    AvailableTimeZones = await GetAvailableTimeZonesAsync(),
                    AvailableDateTimeFormat = GetAvailableDateTimeFormats(),
                    AvailableCultures = await GetAvailableCulturesAsync(),
                    AvailableThemes = GetAvailableThemes(),
                };
            }
            
            // return default settings
            return new SiteSettingsViewModel()
            {
                SiteName = "Example Site"
            };

        }

        #endregion

        #region "Private Methods"
        
        IEnumerable<SelectListItem> GetAvailableDateTimeFormats()
        {
            
            var formats = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            foreach (var value in DateTimeFormats.Defaults)
            {
                formats.Add(new SelectListItem
                {
                    Text = System.DateTime.UtcNow.ToString(value),
                    Value = value
                });
            }

            return formats;

        }

        async Task<IEnumerable<SelectListItem>> GetAvailableTimeZonesAsync()
        {
            // Build timezones 
            var timeZones = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };
            foreach (var z in await _timeZoneProvider.GetTimeZonesAsync())
            {
                timeZones.Add(new SelectListItem
                {
                    Text = z.DisplayName,
                    Value = z.Id
                });
            }

            return timeZones;
        }

        async Task<IEnumerable<SelectListItem>> GetAvailableCulturesAsync()
        {
            // Build timezones 
            var locales = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            // From all locale descriptors get a unique list of supported culture cdes
            var uniqueLocales = new List<string>();
            var localeDescriptors = await _localeProvider.GetLocalesAsync();
            foreach (var localDescriptor in localeDescriptors)
            {
                if (!uniqueLocales.Contains(localDescriptor.Descriptor.Name))
                {
                    uniqueLocales.Add(localDescriptor.Descriptor.Name);
                }
            }

            foreach (var locale in uniqueLocales)
            {
                locales.Add(new SelectListItem
                {
                    Text = locale,
                    Value = locale
                });
            }

            return locales;
        }


        IEnumerable<SelectListItem> GetAvailableThemes()
        {

            // Build timezones 
            var themes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };
            
            foreach (var theme in _themeLoader.AvailableThemes)
            {
                themes.Add(new SelectListItem
                {
                    Text = theme.Name,
                    Value = theme.FullPath.ToLower()
                });
            }

            return themes;
        }

        #endregion

    }
}
