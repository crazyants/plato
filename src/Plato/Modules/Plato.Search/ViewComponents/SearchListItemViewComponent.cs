using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
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
            SearchIndexOptions searchIndexOpts)
        {

            if (searchIndexOpts == null)
            {
                searchIndexOpts = new SearchIndexOptions();
            }

            var model = new SearchListItemViewModel()
            {
                Entity = entity,
                SearchIndexOptions = searchIndexOpts
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }
     

    }


}

