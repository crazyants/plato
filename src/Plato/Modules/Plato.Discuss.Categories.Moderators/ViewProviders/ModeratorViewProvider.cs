using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Discuss.Categories.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Categories.Moderators.ViewProviders
{
    public class ModeratorViewProvider : BaseViewProvider<Moderator>
    {

        private const string ChannelHtmlName = "channel";

        private readonly ICategoryStore<Category> _channelStore;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public ModeratorViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IModeratorStore<Moderator> moderatorStore,
            ICategoryStore<Category> channelStore, 
            IFeatureFacade featureFacade)
        {
            _moderatorStore = moderatorStore;
            _channelStore = channelStore;
            _featureFacade = featureFacade;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Moderator moderator, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator moderator, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Moderator moderator, IViewProviderContext updater)
        {
            
            // Build input options
            var viewModel = new CategoryInputOptions()
            {
                HtmlName = ChannelHtmlName,
                SelectedCategories = new int[] { moderator.CategoryId  },
                Categories = await GetCategories()
            };

            return Views(
                View<CategoryInputOptions>("Moderation.Channels.Edit.Content", model => viewModel).Zone("sidebar").Order(1)
            );

        }

        public override async Task<bool> ValidateModelAsync(Moderator moderator, IUpdateModel updater)
        {
            var valid = await updater.TryUpdateModelAsync(new CategoryInputViewModel()
            {
                SelectedCategories = GetChannelsToAdd()
            });

            return valid;
        }

        public override async Task ComposeModelAsync(Moderator moderator, IUpdateModel updater)
        {

            var model = new CategoryInputViewModel
            {
                SelectedCategories = GetChannelsToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                moderator.CategoryId = model.SelectedCategories.FirstOrDefault();
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Moderator model, IViewProviderContext context)
        {

            var moderator = await _moderatorStore.GetByIdAsync(model.Id);
            if (moderator == null)
            {
                return await BuildIndexAsync(model, context);
            }

            // Validate model
            if (await ValidateModelAsync(model, context.Updater))
            {

                // Get all moderators
                var moderators = await _moderatorStore
                    .QueryAsync()
                    .ToList();

                // Get selected channels
                var selectedCategories = GetChannelsToAdd();

                var output = new List<Moderator>();
                foreach (var categoryId in selectedCategories)
                {

                    // Does the moderator already exists for the category?
                    var existingModerator =
                        moderators.Data.FirstOrDefault(m =>
                            m.UserId == moderator.UserId && m.CategoryId == categoryId);

                    Moderator newOrUpdatedModerator = null;
                    moderator.CategoryId = categoryId;
                    moderator.Claims = GetPostedClaims();

                    // If so update existing moderator
                    if (existingModerator != null)
                    {
                        moderator.Id = existingModerator.Id;
                        newOrUpdatedModerator = await _moderatorStore.UpdateAsync(moderator);
                    }
                    else
                    {
                        moderator.Id = 0;
                        newOrUpdatedModerator = await _moderatorStore.CreateAsync(moderator);
                    }

                    if (newOrUpdatedModerator != null)
                    {
                        output.Add(newOrUpdatedModerator);
                    }

                }

                // If we have channels, lets delete the "All Channels" entry
                if (output.Count > 0)
                {
                    var allChannels = moderators?.Data.FirstOrDefault(m =>
                        m.UserId == moderator.UserId && m.CategoryId == 0);
                    if (allChannels != null)
                    {
                        await _moderatorStore.DeleteAsync(allChannels);
                    }
                }


            }

            return await BuildEditAsync(moderator, context);

        }

        #endregion

        #region "Private Methods"


        IList<ModeratorClaim> GetPostedClaims()
        {
            // Build a list of claims to add or update
            var moderatorClaims = new List<ModeratorClaim>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith("Checkbox.") && _request.Form[key] == "true")
                {
                    var permissionName = key.Substring("Checkbox.".Length);
                    moderatorClaims.Add(new ModeratorClaim { ClaimType = ModeratorPermission.ClaimTypeName, ClaimValue = permissionName });
                }
            }

            return moderatorClaims;

        }

        List<int> GetChannelsToAdd()
        {
            // Build selected channels
            List<int> channelsToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ChannelHtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (id > 0)
                        {
                            if (channelsToAdd == null)
                            {
                                channelsToAdd = new List<int>();
                            }
                            if (!channelsToAdd.Contains(id))
                            {
                                channelsToAdd.Add(id);
                            }
                        }

                    }
                }
            }

            return channelsToAdd;
        }

        async Task<IEnumerable<ICategory>> GetCategories()
        {
            // Get feature
            var featureId = "Plato.Discuss.Categories";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);

            // Ensure feature exists
            if (feature == null)
            {
                throw new Exception($"A feature named '{featureId}' could not be found!");
            }

            // Get categories for feature
            return await _channelStore.GetByFeatureIdAsync(feature.Id);

        }

        #endregion

    }

}
