using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Plato.Internal.Hosting.Extensions
{
    public static class RouteBuilderExtensions
    {
 
        //public static IRouteBuilder MapAreaRoute(
        //    this IRouteBuilder routeBuilder,
        //    string name,
        //    string area,
        //    string template,
        //    string controller,
        //    string action)
        //{

        //    if (routeBuilder == null)
        //        throw new ArgumentNullException(nameof(routeBuilder));
            
        //    if (string.IsNullOrEmpty(area))
        //        throw new ArgumentException(nameof(area));
            
        //    var defaultsDictionary = new RouteValueDictionary
        //    {
        //        ["area"] = area,
        //        ["controller"] = controller,
        //        ["action"] = action
        //    };

        //    var constraintsDictionary = new RouteValueDictionary
        //    {
        //        ["area"] = new StringRouteConstraint(area)
        //    };

        //    routeBuilder.MapRoute(
        //        name, 
        //        template, 
        //        defaultsDictionary, 
        //        constraintsDictionary, 
        //        null);

        //    return routeBuilder;
        //}

        //private class StringRouteConstraint : IRouteConstraint
        //{
        //    private readonly string _value;

        //    public StringRouteConstraint(string value)
        //    {
        //        if (value == null)
        //            throw new ArgumentNullException(nameof(value));
        //        _value = value;
        //    }

   
        //    public bool Match(
        //        HttpContext httpContext, 
        //        IRouter route, 
        //        string routeKey, 
        //        RouteValueDictionary values,
        //        RouteDirection routeDirection)
        //    {
        //        object routeValue;
        //        if (values.TryGetValue(routeKey, out routeValue) && routeValue != null)
        //        {
        //            var parameterValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
        //            return _value.Equals(parameterValueString, StringComparison.OrdinalIgnoreCase);
        //        }
        //        return false;
        //    }
        //}
    }
}
