using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Users.StopForumSpam.Models;
using Plato.Users.StopForumSpam.Stores;
using Plato.Users.StopForumSpam.ViewModels;

namespace Plato.Users.StopForumSpam.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<StopForumSpamSettings>
    {

        private readonly IStopForumSpamSettingsStore<StopForumSpamSettings> _recaptchaSettingsStore;
        private readonly ISpamOperationsManager<SpamOperation> _spamOperationsManager;
        private readonly HttpRequest _request;

        public AdminViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStopForumSpamSettingsStore<StopForumSpamSettings> recaptchaSettingsStore,
            ISpamOperationsManager<SpamOperation> spamOperationsManager)
        {
            _recaptchaSettingsStore = recaptchaSettingsStore;
            _spamOperationsManager = spamOperationsManager;
            _request = httpContextAccessor.HttpContext.Request;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(StopForumSpamSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildIndexAsync(StopForumSpamSettings viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override  async Task<IViewProviderResult> BuildEditAsync(StopForumSpamSettings settings, IViewProviderContext context)
        {
            var recaptchaSettingsViewModel = new StopForumSpamSettingsViewModel()
            {
                ApiKey = settings.ApiKey,
                SpamLevel = settings.SpamLevel,
                SpamOperations = settings.SpamOperations ??
                                 _spamOperationsManager.GetSpamOperations(),
                CategorizedSpamOperations = await _spamOperationsManager.GetCategorizedSpamOperationsAsync()
            };

            return Views(
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Header", model => recaptchaSettingsViewModel).Zone("header").Order(1),
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Tools", model => recaptchaSettingsViewModel).Zone("tools").Order(1),
                View<StopForumSpamSettingsViewModel>("Admin.Edit.Content", model => recaptchaSettingsViewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(StopForumSpamSettings settings,
            IViewProviderContext context)
        {

            // All possible spam operations
            var spamOperations = _spamOperationsManager.GetSpamOperations();
            
            // Build operations to add
            var operationsToAdd = new ConcurrentDictionary<string, SpamOperation>();
            foreach (var operation in spamOperations)
            {

                operation.FlagAsSpam = false;
                operation.NotifyAdmin = false;
                operation.NotifyStaff = false;
                operation.AllowAlter = false;

                foreach (var key in _request.Form.Keys)
                {
                    if (key.EndsWith(operation.Name))
                    {
                        var values = _request.Form[key];
                        foreach (var value in values)
                        {
                            switch (value)
                            {
                                case "flagAsSpam":
                                    operation.FlagAsSpam = true;
                                    break;
                                case "notifyAdmin":
                                    operation.NotifyAdmin = true;
                                    break;
                                case "notifyStaff":
                                    operation.NotifyStaff = true;
                                    break;
                                case "allowAlter":
                                    operation.AllowAlter = true;
                                    break;
                            }
                        }
                    }

                    if (key.StartsWith("customMessage") && key.EndsWith(operation.Name))
                    {
                        var values = _request.Form[key];
                        foreach (var value in values)
                        {
                            operation.Message = value;
                        }
                    }

                    // Ensure unique entries
                    operationsToAdd.AddOrUpdate(operation.Name, operation, (k, v) => operation);
                }
            }
         
            // Build model
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
                    ApiKey = settings.ApiKey,
                    SpamLevel = settings.SpamLevel,
                    SpamOperations = operationsToAdd.Values
                };

                // Persist settings
                await _recaptchaSettingsStore.SaveAsync(settings);

            }

            // Redirect back to edit
            return await BuildEditAsync(settings, context);

        }

    }

}
