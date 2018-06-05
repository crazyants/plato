using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout.Views
{


    public interface IGenericView
    {
        string Name { get; set; }

        object Model { get; set; }
    }

    public class GenericView : IGenericView
    {
        public string Name { get; set; }
        
        public object Model { get; set; }

        public GenericView(string name, object model)
        {
            this.Name = name;
            this.Model = model;
        }

    }
}
