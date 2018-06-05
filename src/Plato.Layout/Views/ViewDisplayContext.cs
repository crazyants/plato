using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Layout.Views
{
    public class ViewDisplayContext
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IViewHelper DisplayAsync { get; set; }
        public ViewContext ViewContext { get; set; }
        public object Value { get; set; }
    }
}
