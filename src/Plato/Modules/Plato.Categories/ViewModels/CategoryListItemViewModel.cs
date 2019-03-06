using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{
    public class CategoryListItemViewModel<TModel> where TModel : class, ICategory
    {

        public TModel Category { get; set; }

        public CategoryIndexOptions Options { get; set; }
        
    }

}
