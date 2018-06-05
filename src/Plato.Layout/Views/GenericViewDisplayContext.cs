using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Layout.Views
{
    public class GenericViewDisplayContext
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IViewDisplayHelper DisplayAsync { get; set; }
        public ViewContext ViewContext { get; set; }
        public object Value { get; set; }

        public GenericViewDescriptor ViewDescriptor { get; set; }
    }
}
