using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Layout.Views
{
    public class GenericViewDescriptor
    {

        public IGenericView View { get; set; }

        public ViewContext Viewcontext { get; set; }
        
    }
}
