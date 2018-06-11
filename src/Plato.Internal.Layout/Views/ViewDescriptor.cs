using System.Collections.Generic;
using Plato.Internal.Layout.ViewAdaptors;

namespace Plato.Internal.Layout.Views
{
    public class ViewDescriptor
    {

        public string Name { get; set; }

        public IView View { get; set; }
        
        public IEnumerable<IViewAdaptorResult> ViewAdaptorResults { get; set; }

        public int Priority { get; set; }
        
    }

}
