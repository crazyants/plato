using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout
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
