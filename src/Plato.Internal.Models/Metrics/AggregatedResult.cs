using System;
using System.Data;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Metrics
{

    public class AggregatedResult<T> where T : struct
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

        public string SerializeLabels()
        {
            var output = new List<string>();
            foreach (var item in Data)
            {

                string value = null;
                if (item.Aggregate is DateTimeOffset)
                {
                    value = ((DateTimeOffset)Convert.ChangeType(item.Aggregate, typeof(DateTimeOffset))).ToPrettyDate();
                }

                if (value == null)
                {
                    value = item.Aggregate.ToString(); ;
                }
                    
                output.Add(value);

            }

            return output.Serialize<string>();

        }

        public string SerializeCounts()
        {
            var output = new List<int>();
            foreach (var item in Data)
            {
                output.Add(item.Count);
            }
            return output.Serialize<int>();
        }
    }

    public class AggregatedCount<T> where T : struct
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
