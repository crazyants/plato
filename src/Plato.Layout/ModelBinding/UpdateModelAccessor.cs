using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout.ModelBinding
{

    public interface IUpdateModelAccessor
    {
        IUpdateModel ModelUpdater { get; set; }
    }

    public class UpdateModelAccessor
    {
    }

}
