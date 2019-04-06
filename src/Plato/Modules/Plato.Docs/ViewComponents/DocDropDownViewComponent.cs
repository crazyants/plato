using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Stores;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;

namespace Plato.Docs.ViewComponents
{

    public class DocDropDownViewComponent : ViewComponent
    {
        private readonly IEntityStore<Entity> _entityStore;
        
        public DocDropDownViewComponent(IEntityStore<Entity> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityDropDownViewModel model)
        {

            if (model == null)
            {
                model = new EntityDropDownViewModel();
            }
            
            model.Entities = await BuildSelectionsAsync(model);
            return View(model);

        }

        private async Task<IList<Selection<Entity>>> BuildSelectionsAsync(EntityDropDownViewModel model)
        {

            if (model.Options.FeatureId == null)
            {
                throw new ArgumentNullException(nameof(model.Options.FeatureId));
            }

            var entities = await _entityStore.GetByFeatureIdAsync(model.Options.FeatureId.Value);
            var selections = entities?.Select(e => new Selection<Entity>
                {
                    IsSelected = model.SelectedEntity.Equals(e.Id),
                    Value = e
                })
                .ToList();
            return selections;
        }
    }
    
}

