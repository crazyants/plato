using System.Threading.Tasks;
using Plato.Facebook.Models;
using Plato.Facebook.Stores;
using Plato.Facebook.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;

namespace Plato.Facebook.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<FacebookSettings>
    {

        private readonly IShellSettings _shellSettings;
        private readonly IFacebookSettingsStore<FacebookSettings> _facebookSettingsStore;
        private readonly IPlatoHost _platoHost;

        public AdminViewProvider(
            IFacebookSettingsStore<FacebookSettings> facebookSettingsStore,
            IPlatoHost platoHost,
            IShellSettings shellSettings)
        {
            _facebookSettingsStore = facebookSettingsStore;
            _platoHost = platoHost;
            _shellSettings = shellSettings;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(FacebookSettings entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(FacebookSettings entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(FacebookSettings entity, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<FacebookSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<FacebookSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(FacebookSettings emailSettings, IViewProviderContext context)
        {
            
            var model = new FacebookSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(emailSettings, context);
            }
            
            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                emailSettings = new FacebookSettings()
                {
                    AppId = model.AppId
                };

                var result = await _facebookSettingsStore.SaveAsync(emailSettings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }
              
            }

            return await BuildEditAsync(emailSettings, context);

        }
        
        async Task<FacebookSettingsViewModel> GetModel()
        {

            var settings = await _facebookSettingsStore.GetAsync();
            if (settings != null)
            {
                return new FacebookSettingsViewModel()
                {
                  AppId = settings.AppId
                };
            }

            // return default settings
            return new FacebookSettingsViewModel()
            {
                AppId = string.Empty
            };

        }
        
    }

}
