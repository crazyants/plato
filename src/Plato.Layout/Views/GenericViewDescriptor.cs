using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Layout.Adaptors;

namespace Plato.Layout.Views
{
    public class GenericViewDescriptor
    {

        public string Name { get; set; }

        public object Value { get; set; }
        
        public IEnumerable<IViewAdaptorResult> ViewAdaptorResults { get; set; }

        public int Priority { get; set; }
        
    }

}
