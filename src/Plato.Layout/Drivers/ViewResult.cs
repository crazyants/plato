using System;
using System.Collections.Generic;
using System.Text;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{

    public interface IViewResult
    {

        IEnumerable<IGenericView> Views { get; set; }

    }

    public class ViewResult : IViewResult
    {
        public IEnumerable<IGenericView> Views { get; set; }

    }
}
