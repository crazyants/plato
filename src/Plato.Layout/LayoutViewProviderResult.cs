using System;
using System.Collections.Generic;
using System.Text;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;

namespace Plato.Layout
{
    public class LayoutViewProviderResult : CombinedViewProviderResult
    {
        private readonly IEnumerable<IViewProviderResult> _results;

        private IEnumerable<IView> _header;
        private IEnumerable<IView> _meta;
        private IEnumerable<IView> _content;
        private IEnumerable<IView> _sidebar;
        private IEnumerable<IView> _footer;
   
        public LayoutViewProviderResult(params IViewProviderResult[] results)
        {
            _results = results;
        }
        
        public IEnumerable<IView> Header
        {
            get => _header ?? (_header = new List<IView>());
            set => _header = value;
        }

        public IEnumerable<IView> Meta
        {
            get => _meta ?? (_meta = new List<IView>());
            set => _meta = value;
        }

        public IEnumerable<IView> Content
        {
            get => _content ?? (_content = new List<IView>());
            set => _content = value;
        }
        
        public IEnumerable<IView> SideBar
        {
            get => _sidebar ?? (_sidebar = new List<IView>());
            set => _sidebar = value;
        }
        
        public IEnumerable<IView> Footer
        {
            get => _footer ?? (_footer = new List<IView>());
            set => _footer = value;
        }


    }

}
