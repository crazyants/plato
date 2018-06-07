using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Layout.Adaptors;

namespace Plato.Layout.Views
{
    public class GenericViewDisplayContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public IViewDisplayHelper DisplayAsync { get; set; }

        public ViewContext ViewContext { get; set; }

        public IEnumerable<IViewAdaptorResult> ViewAdaptorResults { get; set; }

        public object Value { get; set; }
        
        public GenericViewDescriptor ViewDescriptor { get; set; }
    }
}
