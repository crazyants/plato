using System;

namespace Plato.Internal.Layout.Views
{
    public class ViewPosition
    {

        public string Zone { get; set; } 

        public int Order { get; set; }

        public ViewPosition(string zoneName, int order)
        {
            this.Zone = zoneName;
            this.Order = order;
        }

    }
}
