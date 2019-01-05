using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Categories.ViewComponents
{
    public class CategoryTreeViewComponent : ViewComponent
    {
        private readonly ICategoryStore<CategoryBase> _channelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;

        public CategoryTreeViewComponent(
            ICategoryStore<CategoryBase> channelStore,
            IContextFacade contextFacade, 
            IFeatureFacade featureFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            CategoryTreeOptions options)
        {

            if (options == null)
            {
                options = new CategoryTreeOptions();
            }

            if (options.SelectedCategories == null)
            {
                options.SelectedCategories = new int[0];
            }

            return View(new CategoryTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedCategories = await BuildSelectionsAsync(options.SelectedCategories),
                CssClass = options.CssClass
            });

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return channels?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = selected.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }
    }

}
