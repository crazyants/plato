using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityDropDownViewModel : EntityInputViewModel
    {

        public EntityIndexOptions Options { get; set; } = new EntityIndexOptions();

    }

    public class EntityInputViewModel
    {
        
        [Display(Name = "entity")]
        public int SelectedEntity { get; set; }

        public string HtmlName { get; set; }

        public IList<Selection<Entity>> Entities { get; set; }

        public EntityInputViewModel()
        {
        }

        public EntityInputViewModel(IList<Selection<Entity>> entities)
        {
            Entities = entities;
        }
    }

}
