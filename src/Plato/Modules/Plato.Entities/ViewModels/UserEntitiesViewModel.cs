using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Entities.Stores;

namespace Plato.Entities.ViewModels
{
    public class UserEntitiesViewModel
    {

        public FeatureEntityMetrics Metrics { get; set; }

        public EntityIndexViewModel<Entity> IndexViewModel { get; set; }

    }
}
