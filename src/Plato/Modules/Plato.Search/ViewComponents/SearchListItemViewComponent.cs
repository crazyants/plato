using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListItemViewComponent : ViewComponent
    {

        public SearchListItemViewComponent()
        {
        
        }

        public Task<IViewComponentResult> InvokeAsync(
            Entity entity,
            EntityIndexOptions options)
        {

            if (options == null)
            {
                options = new EntityIndexOptions();
            }

            var model = new SearchListItemViewModel()
            {
                Entity = entity,
                SearchIndexOptions = options
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }

    }
    
}

