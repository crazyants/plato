using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Models.Users;

namespace Plato.Entities.ViewModels
{
    public class UserEntitiesViewModel
    {

        public IUser User { get; set; }

        public FeatureEntityMetrics Metrics { get; set; }

        public EntityIndexViewModel<Entity> IndexViewModel { get; set; }

    }
}
