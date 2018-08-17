using System;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        
        public static DateTimeOffset ToDateIfNull(this DateTimeOffset? d)
        {
            if (d == null)
            {
                return DateTimeOffset.UtcNow;
            }

            return (DateTimeOffset)d;

        }

        public static string ToPrettyDate(this DateTimeOffset? d)
        {

            if (d == null)
            {
                return string.Empty;
            }

            return ToPrettyDate((DateTimeOffset)d);
        }

        public static string ToPrettyDate(this DateTimeOffset d)
        {
            return d.DateTime.ToPrettyDate();
        }
    }

}