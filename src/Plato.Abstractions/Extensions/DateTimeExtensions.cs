using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Abstractions.Extensions
{
    public static class DateTimeExtensions
    {

        public static DateTime ToDateIfNull(this DateTime? input)
        {
            return input ?? System.DateTime.Now;
        }
        
    }
}
