using System;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class DateTimeExtensions
    {

        public static DateTime ToDateIfNull(this DateTime? input)
        {
            return input ?? System.DateTime.UtcNow;
        }


        public static string ToPrettyDate(this DateTime? d)
        {
            if (d == null)
            {
                return string.Empty;
            }

            return ToPrettyDate((DateTime) d);

        }


        public static string ToPrettyDate(this DateTime d)
        {
            
            var s = DateTime.Now.Subtract(d);
            var dayDiff = (int)s.TotalDays;
            var secDiff = (int)s.TotalSeconds;

            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            if (dayDiff == 0)
            {
         
                if (secDiff < 60)
                {
                    return "just now";
                }
         
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
         
                if (secDiff < 3600)
                {
                    return $"{Math.Floor((double) secDiff / 60)} minutes ago";
                }
           
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
        
                if (secDiff < 86400)
                {
                    return $"{Math.Floor((double) secDiff / 3600)} hours ago";
                }
            }
   
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return $"{dayDiff} days ago";
            }
            if (dayDiff < 31)
            {
                return $"{Math.Ceiling((double) dayDiff / 7)} weeks ago";
            }
            return null;
        }

    }
}
