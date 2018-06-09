using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout
{
    public class Position
    {

        public string Zone { get; set; }

        public int Order { get; set; }

        public Position(string zoneName, int order)
        {
            this.Zone = zoneName;
            this.Order = order;
        }

    }
}
