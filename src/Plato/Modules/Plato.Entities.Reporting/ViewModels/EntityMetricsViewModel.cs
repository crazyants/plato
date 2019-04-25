using System;
using System.Collections.Generic;
using Plato.Entities.Models;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;

namespace Plato.Entities.Reporting.ViewModels
{
    
    public class TopViewModel
    {

        public IEnumerable<AggregatedModel<int, Entity>> Entities { get; set; }

        public IEnumerable<AggregatedModel<int, User>> Users { get; set; }
        
    }


}
