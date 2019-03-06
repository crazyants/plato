using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListItemViewComponent : ViewComponent
    {
     
        private readonly IContextFacade _contextFacade;

        public ArticleCategoryListItemViewComponent(IContextFacade contextFacade)
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
