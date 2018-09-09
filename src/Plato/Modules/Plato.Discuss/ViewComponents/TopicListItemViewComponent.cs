using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;

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
            Topic topic,
            bool enableEditOptions)
        {
            var model = new TopicListItemViewModel()
            {
                Topic = topic,
                EnableEditOptions = enableEditOptions
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }
     

    }


}

