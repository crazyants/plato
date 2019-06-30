using System.Collections.Generic;

namespace Plato.Categories.Models
{
    public class CategoryInputOptions
    {

        public string HtmlName { get; set; }

        public int[] SelectedCategories { get; set; }

        public IEnumerable<ICategory> Categories { get; set; }
        
    }

}
