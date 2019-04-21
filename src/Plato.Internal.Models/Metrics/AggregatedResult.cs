using System;
using System.Data;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Metrics
{

    public class AggregatedResult<T>  
    {

        public ICollection<AggregatedCount<T>> Data { get; set; } = new List<AggregatedCount<T>>();

        public int Total()
        {

            var output = 0;
            foreach (var item in Data)
            {
                output += item.Count;
            }

            return output;

        }
        
    }

    public class AggregatedCount<T> 
    {

        public T Aggregate { get; set; }

        public int Count { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Aggregate"))
                Aggregate = (T) (dr["Aggregate"]);

            if (dr.ColumnIsNotNull("Count"))
                Count = Convert.ToInt32(dr["Count"]);

        }

    }


}
