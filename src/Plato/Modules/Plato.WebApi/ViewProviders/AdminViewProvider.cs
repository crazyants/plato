using System.Threading.Tasks;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Text.Abstractions;
using Plato.WebApi.Models;
using Plato.WebApi.ViewModels;

namespace Plato.WebApi.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<WebApiSettings>
    {

        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IKeyGenerator _keyGenerator;

        public AdminViewProvider(
            ISiteSettingsStore siteSettingsStore,
            IKeyGenerator keyGenerator)
        {
            _siteSettingsStore = siteSettingsStore;
            _keyGenerator = keyGenerator;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(WebApiSettings entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(WebApiSettings entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(WebApiSettings entity, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<WebApiSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<WebApiSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<WebApiSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(WebApiSettings emailSettings, IViewProviderContext context)
        {
            
            var model = new WebApiSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(emailSettings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Update existing settings
                var settings = await _siteSettingsStore.GetAsync();
                if (settings != null)
                {
                    settings.ApiKey = model.ApiKey;
                }
                else
                {
                    // Create new settings
                    settings = new SiteSettings()
                    {
                        ApiKey = model.ApiKey
                    };
                }

                // Update settings
                var result = await _siteSettingsStore.SaveAsync(settings);

            }

            return await BuildEditAsync(emailSettings, context);

        }

        private async Task<WebApiSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();

            if (settings != null)
            {
                return new WebApiSettingsViewModel()
                {
                    ApiKey = settings.ApiKey

                };
            }

            // return default settings
            return new WebApiSettingsViewModel()
            {
                ApiKey = _keyGenerator.GenerateKey()
            };

        }


    }

}
