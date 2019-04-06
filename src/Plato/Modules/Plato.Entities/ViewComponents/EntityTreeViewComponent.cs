using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery.Internal;
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
            
            // Build selection including parents 
            var selected = await BuildSelectionsAsync(options);
            IEnumerable<Entity> parents = null;
            if (options.SelectedEntity > 0)
            {
                parents = await _entityStore.GetParentsByIdAsync(options.SelectedEntity);
            }
                
            return View(new EntityTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedEntities = selected,
                SelectedParents = parents,
                CssClass = options.CssClass,
                RouteValues = options.RouteValues
            });

        }

        async Task<IList<Selection<Entity>>> BuildSelectionsAsync(
            EntityTreeOptions options)
        {

            if (options.IndexOptions.FeatureId == null)
            {
                throw new ArgumentNullException(nameof(options.IndexOptions.FeatureId));
            }

            // Get entities
            var entities = await _entityStore.GetByFeatureIdAsync(options.IndexOptions.FeatureId.Value);
            
            // Build a model for our tree view
            return entities?.Select(e => new Selection<Entity>
                {
                    IsSelected = options.SelectedEntity.Equals(e.Id),
                    Value = e
                })
                .ToList();
        
        }
        
    }

}
