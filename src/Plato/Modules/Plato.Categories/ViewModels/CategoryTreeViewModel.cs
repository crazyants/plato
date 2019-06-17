using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{
    public class CategoryTreeViewModel
    {
        
        public IList<Selection<CategoryBase>> SelectedCategories { get; set; }

        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }
        
        public RouteValueDictionary RouteValues { get; set; }

    }
    
}
