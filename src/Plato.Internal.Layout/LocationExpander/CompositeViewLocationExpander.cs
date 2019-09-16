using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Internal.Layout.LocationExpander
{
    
    public class CompositeViewLocationExpander : IViewLocationExpanderProvider
    {

        public int Priority
        {
            get { throw new NotSupportedException(); }
        }
        
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var expanderProviders = GetProviders(context);
            foreach (var provider in expanderProviders.OrderBy(x => x.Priority))
            {
                viewLocations = provider.ExpandViewLocations(context, viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var expanderProviders = GetProviders(context);
            foreach (var provider in expanderProviders.OrderBy(x => x.Priority))
            {
                provider.PopulateValues(context);
            }
        }

        private IEnumerable<IViewLocationExpanderProvider> GetProviders(ViewLocationExpanderContext context)
        {
            return context
                .ActionContext
                .HttpContext
                .RequestServices
                .GetServices<IViewLocationExpanderProvider>();
        }
    }

}
