using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ModeratorViewProvider : BaseViewProvider<Moderator>
    {

        private const string ChannelHtmlName = "channel";

        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly HttpRequest _request;


        public ModeratorViewProvider(
            IHttpContextAccessor httpContextAccessor, 
            IModeratorStore<Moderator> moderatorStore)
        {
            _moderatorStore = moderatorStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator moderator, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Moderator moderator, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Moderator moderator, IUpdateModel updater)
        {

            var viewModel = new EditTopicChannelsViewModel()
            {
                HtmlName = ChannelHtmlName,
                SelectedChannels = new List<int>
                {
                    moderator.CategoryId
                }
            };

            return Task.FromResult(Views(
                View<EditTopicChannelsViewModel>("Moderation.Channels.Edit.Content", model => viewModel).Zone("sidebar").Order(1)
            ));

        }


        public override async Task<bool> ValidateModelAsync(Moderator moderator, IUpdateModel updater)
        {
            var valid = await updater.TryUpdateModelAsync(new EditTopicChannelsViewModel()
            {
                SelectedChannels = GetChannelsToAdd()
            });

            return valid;
        }

        public override async Task ComposeTypeAsync(Moderator moderator, IUpdateModel updater)
        {

            var model = new EditTopicChannelsViewModel
            {
                SelectedChannels = GetChannelsToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                moderator.CategoryId = model.SelectedChannels.FirstOrDefault();
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Moderator model, IUpdateModel updater)
        {

            var moderator = await _moderatorStore.GetByIdAsync(model.Id);
            if (moderator == null)
            {
                return await BuildIndexAsync(model, updater);
            }
            
            // Validate model
            if (await ValidateModelAsync(model, updater))
            {

                // Get selected channels
                var selectedChannels = GetChannelsToAdd();

                // Update channel
                moderator.CategoryId = selectedChannels.FirstOrDefault();

                // Persist moderator
                await _moderatorStore.UpdateAsync(moderator);

            }

            return default(IViewProviderResult);
        }

        #endregion
        
        #region "Private Methods"

        List<int> GetChannelsToAdd()
        {
            // Build selected channels
            List<int> channelsToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ChannelHtmlName))
                {
                    if (channelsToAdd == null)
                    {
                        channelsToAdd = new List<int>();
                    }
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (!channelsToAdd.Contains(id))
                        {
                            channelsToAdd.Add(id);
                        }
                    }
                }
            }

            return channelsToAdd;
        }

        #endregion
        
    }

}
