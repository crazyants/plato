using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements
{
    public interface IElementFactory
    {
        T Create<T>(string shapeType) where T : class;
        object Create(Type type, string shapeType);
        T Create<T>(T obj) where T : class;
        IElement Create(string shapeType);
        IElement Create(string shapeType, object parameters);
        IElement Create(string shapeType, object parameters, Func<dynamic> createShape);
        dynamic New { get; }
    }
}
