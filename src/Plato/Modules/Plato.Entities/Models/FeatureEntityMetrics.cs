using System;
using System.Collections.Generic;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Models
{

    public class FeatureEntityCounts
    {
        public IEnumerable<FeatureEntityCount> Features { get; set; }

        public int Total()
        {

            if (Features == null)
            {
                return 0;
            }

            var total = 0;
            foreach (var feature in Features)
            {
                total += feature.Count;
            }

            return total;
        }
    }
    
    public class FeatureEntityCount : IDbModel
    {

        public string ModuleId { get; set; }

        public int Count { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("Count"))
                Count = Convert.ToInt32(dr["Count"]);

        }

    }
}

