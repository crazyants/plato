using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{
    public class CategoryTreeViewModel
    {
        
        public IList<Selection<Category>> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

    }
    
}
