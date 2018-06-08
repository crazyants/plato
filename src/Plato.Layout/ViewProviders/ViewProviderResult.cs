using System.Collections.Generic;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{

    public interface IViewProviderResult
    {
        IList<IView> Views { get; set; }
    }

    public class ViewProviderResult : IViewProviderResult
    {
 
        private IList<IView> _views;

        public IList<IView> Views
        {
            get => _views ?? (_views = new List<IView>());
            set => _views = value;
        }

    }
    
}
