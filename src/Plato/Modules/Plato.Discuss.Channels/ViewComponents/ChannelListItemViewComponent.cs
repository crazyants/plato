using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;

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
            ViewOptions viewOpts)
        {

            if (viewOpts == null)
            {
                viewOpts = new ViewOptions();
            }

            var model = new ChannelListItemViewModel()
            {
                Channel = channel,
                ViewOpts = viewOpts
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }


    }


}
