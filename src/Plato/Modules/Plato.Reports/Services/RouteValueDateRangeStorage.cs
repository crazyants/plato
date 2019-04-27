using System;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Reports.Services
{
    
    public class RouteValueDateRangeStorage : IDateRangeStorage
    {

        private DateTimeOffset _start = DateTimeOffset.UtcNow.AddDays(-7);
        private DateTimeOffset _end = DateTimeOffset.UtcNow;

        public DateTimeOffset Start => _start;
        
        public DateTimeOffset End => _end;

        private ActionContext _context;

        private const string ByStartName = "opts.start";
        private const string ByEndName = "opts.end";
        
        public void Set(DateTimeOffset start, DateTimeOffset end)
        {

            if (_context == null)
            {
                throw new Exception("Context cannot be null. You must call Contextualize first passing in a valid ActionContext");
                ;
            }

            _context.RouteData.Values.Add(ByStartName, start.ToString("yyyy/MM/dd"));
            _context.RouteData.Values.Add(ByEndName, end.ToString("yyyy/MM/dd"));
        }

        public RouteValueDateRangeStorage Contextualize(ActionContext context)
        {
            _context = context;

            if (_context.RouteData.Values[ByStartName] != null)
            {
                var ok = DateTime.TryParse(_context.RouteData.Values[ByStartName].ToString(), out var startDate);
                if (ok)
                {
                    _start = startDate;
                }
            }

            if (_context.RouteData.Values[ByEndName] != null)
            {
                var ok = DateTime.TryParse(_context.RouteData.Values[ByEndName].ToString(), out var endDate);
                if (ok)
                {
                    _end = endDate;
                }

            }

            return this;

        }

    }

}
