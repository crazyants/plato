using System.Collections.Generic;

namespace Plato.Categories.Models
{
    public class CategoryTreeOptions
    {

        public IEnumerable<int> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

    }

}
