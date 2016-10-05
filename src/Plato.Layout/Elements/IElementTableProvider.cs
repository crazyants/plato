using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements
{
    public interface IElementTableProvider
    {
        void Discover(ElementTableBuilder builder);

    }
}
