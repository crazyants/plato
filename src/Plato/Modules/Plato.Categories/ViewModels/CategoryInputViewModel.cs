using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Categories.Models;

namespace Plato.Categories.ViewModels
{
    public class CategoryInputViewModel
    {

        [Required]
        [Display(Name = "category")]
        public IEnumerable<int> SelectedCategories { get; set; }
        
        public string HtmlName { get; set; }
        
        public IList<Selection<CategoryBase>> Categories { get; }
        
        public CategoryInputViewModel()
        {
        }

        public CategoryInputViewModel(IList<Selection<CategoryBase>> categories)
        {
            Categories = categories;
        }
    }

    public class Selection<TModel> where TModel : class, ICategory
    {

        public bool IsSelected { get; set; }

        public TModel Value { get; set; }

    }


}
