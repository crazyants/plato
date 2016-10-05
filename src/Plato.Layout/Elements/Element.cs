using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements
{
    public class Element : IElement, IPosition
    {

        private readonly List<IPosition> _items = new List<IPosition>();

        public ElementMetaData MetaData { get; set; }

        public Element()
        {

        }

        public string Position
        {
            get { return MetaData.Position; }
            set { MetaData.Position = value; }
        }

        public virtual Element Add(object item, string position = null)
        {
            if (item == null)
            {
                return this;
            }

            if (position == null)
            {
                position = "";
            }

            //_sorted = false;

            if (item is IHtmlContent)
            {
                _items.Add(new PositionWrapper((IHtmlContent)item, position));
            }
            else if (item is string)
            {
                _items.Add(new PositionWrapper((string)item, position));
            }
            else
            {
                var shape = item as IPosition;
                if (shape != null)
                {
                    if (position != null)
                    {
                        shape.Position = position;
                    }

                    _items.Add(shape);
                }
            }

            return this;
        }


    }
}
