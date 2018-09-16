using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{
    public class CategoryTreeViewModel
    {
        
        public IList<Selection<CategoryBase>> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }


}
