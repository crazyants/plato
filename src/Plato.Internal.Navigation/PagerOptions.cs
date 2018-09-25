using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Navigation
{
    public class PagerOptions
    {

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        private int _total;

        public int Total => _total;

        public void SetTotal(int total)
        {
            _total = total;
        }

        public PagerOptions()
        {
        }
        
        public PagerOptions(RouteData routeData)
        {
            Page = GetRouteValueOrDefault<int>("pager.page", routeData, Page);
            PageSize = GetRouteValueOrDefault<int>("pager.size", routeData, PageSize);
        }

        private T GetRouteValueOrDefault<T>(string key, RouteData routeData, T defaultValue)
        {

            if (routeData == null)
            {
                return defaultValue;
            }

            var found = routeData.Values.TryGetValue(key, out object value);
            if (found)
            {
                return (T)value;
            }
            return defaultValue;
        }


    }
}
