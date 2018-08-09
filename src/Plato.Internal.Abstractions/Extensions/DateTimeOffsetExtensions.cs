using System;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToPrettyDate(this DateTimeOffset d)
        {
            return d.DateTime.ToPrettyDate();
        }
    }

}