using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Slack.Configuration;
using Plato.Slack.Models;
using Plato.Slack.Stores;
using Plato.Slack.ViewModels;

namespace Plato.Slack.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<SlackSettings>
    {

        private readonly ISlackSettingsStore<SlackSettings> _TwitterSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            ISlackSettingsStore<SlackSettings> TwitterSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _TwitterSettingsStore = TwitterSettingsStore;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(SlackSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(SlackSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(SlackSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<SlackSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<SlackSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<SlackSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(SlackSettings settings, IViewProviderContext context)
        {
            
            var model = new SlackSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var webHookUrl = string.Empty;
                var accessTokenSecret = string.Empty;

                if (!string.IsNullOrWhiteSpace(model.WebHookUrl))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(SlackOptionsConfiguration));
                        webHookUrl = protector.Protect(model.WebHookUrl);
               
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                // Create the model
                settings = new SlackSettings()
                {
                    WebHookUrl = webHookUrl
                };

                // Persist the settings
                var result = await _TwitterSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }
              
            }

            return await BuildEditAsync(settings, context);

        }
        
        async Task<SlackSettingsViewModel> GetModel()
        {

            var settings = await _TwitterSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var webHookUrl = string.Empty;
         
                if (!string.IsNullOrWhiteSpace(settings.WebHookUrl))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(SlackOptionsConfiguration));
                        webHookUrl = protector.Unprotect(settings.WebHookUrl);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the Twitter app secret. {e.Message}");
                    }
                }

                return new SlackSettingsViewModel()
                {
                    WebHookUrl = webHookUrl
                };

            }

            // return default settings
            return new SlackSettingsViewModel();

        }
        
    }

}
