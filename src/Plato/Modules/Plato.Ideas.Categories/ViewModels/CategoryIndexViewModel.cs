using Plato.Categories.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Categories.ViewModels
{
    public class CategoryIndexViewModel 
    {
        
        public PagerOptions Pager { get; set; }

        public CategoryIndexOptions Options { get; set; }

        public EntityIndexOptions EntityIndexOptions { get; set; }
        
    }

}
