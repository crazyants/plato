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
 
        public ChannelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Channel channel, CategoryIndexOptions categoryIndexOpts)
        {

            if (categoryIndexOpts == null)
            {
                categoryIndexOpts = new CategoryIndexOptions();
            }

            var model = new ChannelListItemViewModel()
            {
                Channel = channel,
                Options = categoryIndexOpts
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}
