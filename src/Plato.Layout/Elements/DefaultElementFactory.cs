using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements
{
    public class DefaultElementFactory
    {
        IElementTableManager _elementTableManager;

        public DefaultElementFactory(
          IElementTableManager elementTableManager)
        {
       
            _elementTableManager = elementTableManager;
          
        }
        
        public dynamic New { get { return this; } }






        //public IElement Create(string shapeType, object parameters, Func<dynamic> createShape)
        //{

        //    var elementTable = _elementTableManager.GetElementTable("classic"); 


        //}



    }
}
