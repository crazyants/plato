using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;
using Plato.Facebook.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Facebook.Configuration;

namespace Plato.Facebook.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<FacebookSettings>
    {

        private readonly IFacebookSettingsStore<FacebookSettings> _facebookSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            IFacebookSettingsStore<FacebookSettings> facebookSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _facebookSettingsStore = facebookSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(FacebookSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(FacebookSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(FacebookSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<FacebookSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(FacebookSettings settings, IViewProviderContext context)
        {
            
            var model = new FacebookSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.AppSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(FacebookOptionsConfiguration));
                        secret = protector.Protect(model.AppSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Facebook app secret. {e.Message}");
                    }
                }
                
                // Create the model
                settings = new FacebookSettings()
                {
                    AppId = model.AppId,
                    AppSecret = secret
                };

                // Persist the settings
                var result = await _facebookSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }
              
            }

            return await BuildEditAsync(settings, context);

        }
        
        async Task<FacebookSettingsViewModel> GetModel()
        {

            var settings = await _facebookSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(settings.AppSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(FacebookOptionsConfiguration));
                        secret = protector.Unprotect(settings.AppSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Facebook app secret. {e.Message}");
                    }
                }


                return new FacebookSettingsViewModel()
                {
                  AppId = settings.AppId,
                  AppSecret = secret
                };
            }

            // return default settings
            return new FacebookSettingsViewModel()
            {
                AppId = string.Empty,
                AppSecret = string.Empty
            };

        }
        
    }

}
