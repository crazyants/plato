using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Articles.ViewComponents
{
    public class TopicListItemViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
   
        public TopicListItemViewComponent(IContextFacade contextFacade, IEntityStore<Article> entityStore)
        {
            _contextFacade = contextFacade;
        }

        public Task<IViewComponentResult> InvokeAsync(
            TopicListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

