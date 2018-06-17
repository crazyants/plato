using System.Collections.Generic;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout
{
    public class LayoutViewModel : CombinedViewProviderResult
    {
    
        public IEnumerable<IPositionedView> Header { get; set; }

        public IEnumerable<IPositionedView> Tools { get; set; }
        
        public IEnumerable<IPositionedView> Meta { get; set; }

        public IEnumerable<IPositionedView> Content { get; set; }
        
        public IEnumerable<IPositionedView> SideBar { get; set; }

        public IEnumerable<IPositionedView> Footer { get; set; }

        public IEnumerable<IPositionedView> Actions { get; set; }

        public IEnumerable<IPositionedView> Asides { get; set; }

        public IEnumerable<IPositionedView> Alerts { get; set; }
        
        public LayoutViewModel(params IViewProviderResult[] results) : base(results)
        {
        }
        
        public LayoutViewModel BuildLayout()
        {
            return new LayoutComposition(base.GetResults()).Compose();
        }
        
    }

}
