using System.Collections.Generic;
using Plato.Entities.ViewModels;

namespace Plato.Entities.Models
{
    public class EntityTreeOptions
    {

        public EntityIndexOptions IndexOptions { get; set; } = new EntityIndexOptions();

        public IEnumerable<int> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

    }

}
