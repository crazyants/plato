using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Moderation.Models;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ModeratorViewProvider : BaseViewProvider<Moderator>
    {


        private const string ChannelHtmlName = "channel";

        private readonly HttpRequest _request;


        public ModeratorViewProvider(
            IHttpContextAccessor httpContextAccessor)
        {
            _request = httpContextAccessor.HttpContext.Request;
        }

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
                SelectedChannels = null
            };

            return Task.FromResult(Views(
                View<EditTopicChannelsViewModel>("Moderation.Channels.Edit.Content", model => viewModel).Zone("sidebar").Order(1)
            ));


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
             
            }

        }

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



        public override Task<IViewProviderResult> BuildUpdateAsync(Moderator moderator, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }



    }
}
