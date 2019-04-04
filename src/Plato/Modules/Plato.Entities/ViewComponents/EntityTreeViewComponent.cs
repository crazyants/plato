using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Categories.ViewComponents
{
    public class EntityTreeViewComponent : ViewComponent
    {

        private readonly IEntityStore<Entity> _categoryStore;
        
        public EntityTreeViewComponent(
            IEntityStore<Entity> categoryStore)
        {
            _categoryStore = categoryStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityTreeOptions options)
        {

            if (options == null)
            {
                options = new EntityTreeOptions();
            }

            if (options.SelectedCategories == null)
            {
                options.SelectedCategories = new int[0];
            }

            var selected = await BuildSelectionsAsync(options);
            return View(new EntityTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedCategories = selected,
                CssClass = options.CssClass
            });

        }

        private async Task<IList<Selection<Entity>>> BuildSelectionsAsync(EntityTreeOptions options)
        {

            if (options.IndexOptions.FeatureId == null)
            {
                throw new ArgumentNullException(nameof(options.IndexOptions.FeatureId));
            }

            var channels = await _categoryStore.GetByFeatureIdAsync(options.IndexOptions.FeatureId.Value);

            return channels?.Select(c => new Selection<Entity>
                {
                    IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }
    }

}
