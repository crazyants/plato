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

            if (d.Value == DateTimeOffset.MinValue)
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

        public static int DayDifference(this DateTimeOffset input, DateTime date)
        {
            return input.DateTime.DayDifference(date);
        }

        public static int DayDifference(this DateTimeOffset input, DateTimeOffset date)
        {
            return input.DayDifference(date.DateTime);
        }

        public static int DayDifference(this DateTimeOffset input, DateTimeOffset? date)
        {
            if (date == null)
            {
                return 0;
            }

            return input.DayDifference(date.Value.DateTime);
        }

        public static string ToSortableDateTimePattern(this DateTimeOffset input)
        {
            return input.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
        }

        public static DateTimeOffset Floor(this DateTimeOffset d)
        {
            return d.DateTime.Floor();
        }

        public static DateTimeOffset Ceil(this DateTimeOffset d)
        {
            return d.DateTime.Ceil();
        }

    }

}