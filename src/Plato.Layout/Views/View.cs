using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout.Views
{
    public class View
    {
        public string Name { get; set; }
        
        public object Model { get; set; }

        public View(string name, object model)
        {
            this.Name = name;
            this.Model = model;
        }

    }
}
