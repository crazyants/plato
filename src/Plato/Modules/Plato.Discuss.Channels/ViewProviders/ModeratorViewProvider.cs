using System;
using System.Threading.Tasks;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Moderation.Models;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ModeratorViewProvider : BaseViewProvider<Moderator>
    {


        private const string ChannelHtmlName = "channel";


        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator moderator, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Moderator moderator, IUpdateModel updater)
        {
            throw new NotImplementedException();
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

        public override Task<IViewProviderResult> BuildUpdateAsync(Moderator moderator, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }
}
