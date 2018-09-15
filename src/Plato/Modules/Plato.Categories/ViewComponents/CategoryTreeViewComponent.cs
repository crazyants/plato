using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Categories.ViewComponents
{
    public class CategoryTreeViewComponent : ViewComponent
    {
        private readonly ICategoryStore<CategoryBase> _channelStore;
        private readonly IContextFacade _contextFacade;

        public CategoryTreeViewComponent(
            ICategoryStore<CategoryBase> channelStore,
            IContextFacade contextFacade)
        {
            _channelStore = channelStore;
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<int> selectedCategories,
            string htmlName,
            bool enableCheckBoxes)
        {
            if (selectedCategories == null)
            {
                selectedCategories = new int[0];
            }

            return View(new CategoryTreeViewModel
            {
                HtmlName = htmlName,
                EnableCheckBoxes = enableCheckBoxes,
                SelectedChannels = await BuildSelectionsAsync(selectedCategories)
            });

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(
            IEnumerable<int> selected)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            var channels = await _channelStore.GetByFeatureIdAsync(feature.Id);

            var selections = channels?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = selected.Any(v => v == c.Id),
                    Value = c
                })
                //.OrderBy(s => s.Value.Name)
                .ToList();

            return selections;
        }
    }

}
