using System;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Reporting
{
    public class RouteValueDateRangeStorage
    {

        public DateTimeOffset Start = DateTimeOffset.UtcNow.AddDays(-7);
        
        public DateTimeOffset End = DateTimeOffset.UtcNow;

        private readonly ControllerContext _context;

        public RouteValueDateRangeStorage(ControllerContext context)
        {
            _context = context;
            
            if (_context.RouteData.Values["start"] != null)
            {
                var ok = DateTime.TryParse(_context.RouteData.Values["start"].ToString(), out var startDate);
                if (ok)
                {
                    Start = startDate;
                }
            }

            if (_context.RouteData.Values["end"] != null)
            {
                var ok = DateTime.TryParse(_context.RouteData.Values["end"].ToString(), out var endDate);
                if (ok)
                {
                    End = endDate;
                }

            }
            
        }
        
        public void Set(DateTimeOffset start, DateTimeOffset end)
        {
            _context.RouteData.Values.Add("start", start.ToString("yyyy/MM/dd"));
            _context.RouteData.Values.Add("end", end.ToString("yyyy/MM/dd"));
        }
        
    }

}
