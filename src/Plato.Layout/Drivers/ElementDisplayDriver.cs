using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;
using Plato.Layout.Views;

namespace Plato.Layout.Display
{
    public class ElementDisplayDriver
    {


        public ElementResult Shape(string shapeType, object model)
        {
            return new ElementResult(shapeType, 
                ctx => ctx.ElementFactory.Create(shapeType, model));
        }

    }
}
