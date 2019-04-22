using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Metrics
{

    public class AggregatedCount<T> : IDbModel
    {

        public T Aggregate { get; set; }

        public int Count { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Aggregate"))
                Aggregate = (T)(dr["Aggregate"]);

            if (dr.ColumnIsNotNull("Count"))
                Count = Convert.ToInt32(dr["Count"]);

        }

    }

}
