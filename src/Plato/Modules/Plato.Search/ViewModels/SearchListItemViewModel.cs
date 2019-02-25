using Plato.Entities.Models;
using Plato.Entities.ViewModels;

namespace Plato.Search.ViewModels
{
    public class SearchListItemViewModel
    {

        public Entity Entity { get; set; }

        public EntityIndexOptions SearchIndexOptions { get; set; }
        
    }
}
