using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.ViewModels;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Discuss.Channels.ViewComponents
{

    public class ChannelListItemViewComponent : ViewComponent
    {
 
        public ChannelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Channel channel, 
            CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }

            var model = new CategoryListItemViewModel<Channel>()
            {
                Category = channel,
                Options = options
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}
