using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Models;

namespace Plato.Search.ViewModels
{
    public class SearchListItemViewModel
    {

        public Entity Entity { get; set; }
        
        public EntityIndexOptions Options { get; set; }
    
        public ILabelBase Channel { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }

    }

}
