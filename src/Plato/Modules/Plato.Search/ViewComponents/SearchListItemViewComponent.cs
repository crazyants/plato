using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListItemViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
   
        public SearchListItemViewComponent(
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _contextFacade = contextFacade;
        }

        public Task<IViewComponentResult> InvokeAsync(
            Entity entity,
            bool enableEditOptions)
        {
            var model = new SearchListItemViewModel()
            {
                Entity = entity
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }
     

    }


}

