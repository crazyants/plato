using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Layout.ViewAdaptors;

namespace Plato.Layout.Views
{
    public class ViewDescriptor
    {

        public string Name { get; set; }

        public IView View { get; set; }
        
        public IEnumerable<IViewAdaptorResult> ViewAdaptorResults { get; set; }

        public int Priority { get; set; }
        
    }

}
