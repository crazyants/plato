using System.Collections.Generic;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;

namespace Plato.Layout
{
    public class LayoutViewModel : CombinedViewProviderResult
    {
    
        public IEnumerable<IView> Header { get; set; }

        public IEnumerable<IView> Tools { get; set; }
        
        public IEnumerable<IView> Meta { get; set; }

        public IEnumerable<IView> Content { get; set; }
        
        public IEnumerable<IView> SideBar { get; set; }

        public IEnumerable<IView> Footer { get; set; }

        public IEnumerable<IView> Actions { get; set; }

        public IEnumerable<IView> Asides { get; set; }
        
        public LayoutViewModel(params IViewProviderResult[] results) : base(results)
        {
        }
        
        public LayoutViewModel BuildLayout()
        {
            return new LayoutComposition(base.GetResults()).Compose();
        }
        
    }

}
