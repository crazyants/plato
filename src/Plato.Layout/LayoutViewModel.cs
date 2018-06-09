using System;
using System.Collections.Generic;
using System.Text;
using Plato.Layout.Views;

namespace Plato.Layout
{
    public class LayoutViewModel
    {

        public IEnumerable<IView> Header { get; set; }

        public IEnumerable<IView> Meta { get; set; }

        public IEnumerable<IView> Content { get; set; }

        public IEnumerable<IView> SideBar { get; set; }

        public IEnumerable<IView> Footer { get; set; }

    }

}
