using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Plato.Categories.ViewModels;

namespace Plato.Categories.Models
{

    public class CategoryTreeOptions
    {

        public CategoryIndexOptions IndexOptions { get; set; } = new CategoryIndexOptions();

        public IEnumerable<int> SelectedCategories { get; set; }
        
        public string HtmlName { get; set; }

        public bool EnableCheckBoxes { get; set; }

        public string EditMenuViewName { get; set; }

        public string CssClass { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

    }

}
