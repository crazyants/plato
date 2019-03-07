using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Categories.Models;
using Plato.Categories.ViewModels;

namespace Plato.Categories.ViewComponents
{

    public class CategoryDropDownViewComponent : ViewComponent
    {
        private readonly ICategoryStore<CategoryBase> _channelStore;


        public CategoryDropDownViewComponent(ICategoryStore<CategoryBase> channelStore)
        {
            _channelStore = channelStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryDropDownViewModel model)
        {

            if (model == null)
            {
                model = new CategoryDropDownViewModel();
            }

            if (model.SelectedCategories == null)
            {
                model.SelectedCategories = new int[0];
            }
            
            model.Categories = await BuildSelectionsAsync(model);
            return View(model);

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(CategoryDropDownViewModel model)
        {
            var channels = await _channelStore.GetByFeatureIdAsync(model.Options.FeatureId);
            var selections = channels?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = model.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();
            return selections;
        }
    }
    
}

