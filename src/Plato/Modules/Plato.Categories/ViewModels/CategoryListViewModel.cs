using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{

    public class CategoryListViewModel<TModel> where TModel : class, ICategory
    {
    
        public CategoryIndexOptions Options { get; set; } = new CategoryIndexOptions();

        public IEnumerable<TModel> Categories { get; set; }

    }

}
