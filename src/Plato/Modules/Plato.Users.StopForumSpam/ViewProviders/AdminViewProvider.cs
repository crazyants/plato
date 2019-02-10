using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Users.StopForumSpam.Models;
using Plato.Users.StopForumSpam.Stores;
using Plato.Users.StopForumSpam.ViewModels;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<StopForumSpamSettings>
    {

        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _recaptchaSettingsStore;

        public AdminViewProvider(
            IStopForumSpamSettingsStore<StopForumSpamSettings> recaptchaSettingsStore)
        {
            _recaptchaSettingsStore = recaptchaSettingsStore;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(StopForumSpamSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildIndexAsync(StopForumSpamSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildEditAsync(StopForumSpamSettings viewModel, IViewProviderContext context)
        {
            
            var recaptchaSettingsViewModel = new StopForumSpamSettingsViewModel()
            {
                ApiKey = viewModel.ApiKey
            };

            return Task.FromResult(Views(
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Header", model => recaptchaSettingsViewModel).Zone("header").Order(1),
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Tools", model => recaptchaSettingsViewModel).Zone("tools").Order(1),
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Content", model => recaptchaSettingsViewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(StopForumSpamSettings settings,
            IViewProviderContext context)
        {

            var model = new StopForumSpamSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {
                settings = new StopForumSpamSettings()
                {
                    ApiKey = settings.ApiKey
                };

                // Update reCAPTCHA settings
                await _recaptchaSettingsStore.SaveAsync(settings);

            }

            // Redirect back to edit
            return await BuildEditAsync(settings, context);

        }

    }

}
