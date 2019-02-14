using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;
using Plato.StopForumSpam.ViewModels;

namespace Plato.StopForumSpam.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<SpamSettings>
    {

        private readonly ISpamSettingsStore<SpamSettings> _recaptchaSettingsStore;
        private readonly ISpamOperationManager<SpamOperation> _spamOperationManager;
        private readonly HttpRequest _request;

        public AdminViewProvider(
            IHttpContextAccessor httpContextAccessor,
            ISpamSettingsStore<SpamSettings> recaptchaSettingsStore,
            ISpamOperationManager<SpamOperation> spamOperationManager)
        {
            _recaptchaSettingsStore = recaptchaSettingsStore;
            _spamOperationManager = spamOperationManager;
            _request = httpContextAccessor.HttpContext.Request;

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SpamSettings viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(SpamSettings viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override  async Task<IViewProviderResult> BuildEditAsync(SpamSettings settings, IViewProviderContext context)
        {
            var viewModel = new SpamSettingsViewModel()
            {
                ApiKey = settings.ApiKey,
                SpamLevelId = settings.SpamLevelId,
                SpamOperations = settings.SpamOperations ??_spamOperationManager.GetSpamOperations(),
                CategorizedSpamOperations = await _spamOperationManager.GetCategorizedSpamOperationsAsync()
            };

            return Views(
                View<SpamSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<SpamSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<SpamSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(SpamSettings settings,
            IViewProviderContext context)
        {

            // All possible spam operations
            var spamOperations = _spamOperationManager.GetSpamOperations();
            
            // Build operations to add
            var operationsToAdd = new ConcurrentDictionary<string, SpamOperation>();
            foreach (var operation in spamOperations)
            {

                operation.FlagAsSpam = false;
                operation.NotifyAdmin = false;
                operation.NotifyStaff = false;
                operation.CustomMessage = false;

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
                                    operation.CustomMessage = true;
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
            var model = new SpamSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {
                settings = new SpamSettings()
                {
                    ApiKey = settings.ApiKey,
                    SpamLevelId = settings.SpamLevelId,
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
