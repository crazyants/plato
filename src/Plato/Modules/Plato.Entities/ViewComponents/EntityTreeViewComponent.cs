using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

namespace Plato.Entities.ViewComponents
{
    public class EntityTreeViewComponent : ViewComponent
    {

        private readonly IEntityStore<Entity> _entityStore;
        
        public EntityTreeViewComponent(
            IEntityStore<Entity> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityTreeOptions options)
        {

            if (options == null)
            {
                options = new EntityTreeOptions();
            }
            
            var selected = await BuildSelectionsAsync(options);
            return View(new EntityTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedEntities = selected,
                CssClass = options.CssClass,
                RouteValues = options.RouteValues
            });

        }

        private async Task<IList<Selection<Entity>>> BuildSelectionsAsync(EntityTreeOptions options)
        {

            if (options.IndexOptions.FeatureId == null)
            {
                throw new ArgumentNullException(nameof(options.IndexOptions.FeatureId));
            }

            var entity = await _entityStore.GetByFeatureIdAsync(options.IndexOptions.FeatureId.Value);

            return entity?.Select(e => new Selection<Entity>
                {
                    IsSelected = options.SelectedEntity.Equals(e.Id),
                    Value = e
                })
                .ToList();

        }
    }

}
