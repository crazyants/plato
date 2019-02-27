using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListItemViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
   
        public TopicListItemViewComponent(IContextFacade contextFacade, IEntityStore<Topic> entityStore)
        {
            _contextFacade = contextFacade;
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Topic> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

