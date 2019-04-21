using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Metrics;

namespace Plato.Internal.Models.Extensions
{

    public static class AggregatedResultExtensions
    {
        
        public static string SerializeLabels<T>(this AggregatedResult<T> result) where T : struct
        {
            var output = new List<string>();
            if (result.Data != null)
            {
                foreach (var item in result.Data)
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

                }
            return output.Serialize<string>();

        }

        public static string SerializeCounts<T>(this AggregatedResult<T> result) where T : struct
        {
            var output = new List<int>();
            if (result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    output.Add(item.Count);
                }
            }
            return output.Serialize<int>();
        }
        
        public static AggregatedResult<T> MergeIntoRange<T>(this AggregatedResult<T> result, T start, T end) where T : struct
        {

            // Final output
            var output = new AggregatedResult<T>();

            // Pattern match dates
            var startDate = start as DateTimeOffset?;
            var endDate = end as DateTimeOffset?;

            // Are we working with dates
            if (startDate != null && endDate != null)
            {

                // Normalize dates
                var normalizedStartDate = NormalizeDateTimeOffset(startDate.Value);
                var normalizedEndDate = NormalizeDateTimeOffset(endDate.Value);

                // Create dummy range
                var delta = normalizedStartDate.DayDifference(normalizedEndDate);
                for (var i = 0; i <= delta; i++)
                {

                    var aggregate = normalizedStartDate.AddDays(i);
                    var count = 0;

                    // Add real count for dummy aggregate
                    if (result.Data != null)
                    {
                        foreach (var data in result.Data)
                        {
                            var comparer = ((DateTimeOffset)Convert.ChangeType(data.Aggregate, typeof(DateTimeOffset)));
                            if (aggregate.Equals(NormalizeDateTimeOffset(comparer)))
                            {
                                count = data.Count;
                            }
                        }
                    }
                   
                    // Add data to 
                    output.Data.Add(new AggregatedCount<T>()
                    {
                        Aggregate = (T)Convert.ChangeType(aggregate, typeof(T)),
                        Count = count
                    });
                }

            }

            return output;

        }

        static DateTimeOffset NormalizeDateTimeOffset(DateTimeOffset input)
        {
            return new DateTimeOffset(input.Year, input.Month, input.Day,
                0, 0, 0, new TimeSpan());
        }
    }

}
