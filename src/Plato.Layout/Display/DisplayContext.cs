using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Display
{
    public class DisplayContext
    {
        public DisplayHelper Display { get; set; }
        public ViewContext ViewContext { get; set; }
        public object Value { get; set; }
    }
}
