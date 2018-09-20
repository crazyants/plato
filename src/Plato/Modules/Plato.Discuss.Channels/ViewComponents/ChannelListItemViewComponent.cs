using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelListItemViewComponent : ViewComponent
    {
     
        private readonly IContextFacade _contextFacade;

        public ChannelListItemViewComponent(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public Task<IViewComponentResult> InvokeAsync(
            Channel channel,
            ChannelIndexOptions channelIndexOpts)
        {

            if (channelIndexOpts == null)
            {
                channelIndexOpts = new ChannelIndexOptions();
            }

            var model = new ChannelListItemViewModel()
            {
                Channel = channel,
                ChannelIndexOpts = channelIndexOpts
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}
