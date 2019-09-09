using System.Collections.Generic;
using Plato.Internal.Layout.ViewAdapters;

namespace Plato.Internal.Layout.Views
{
    public class ViewDescriptor
    {

        public string Name { get; set; }

        public IView View { get; set; }
      
        public bool IsAnonymousType { get; set; }
    }

}
