using System.Collections.Generic;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{
    public interface IViewProviderResult
    {
        IList<IView> Views { get; }
    }

    public class ViewProviderResult : IViewProviderResult
    {

        private IList<IView> _views;

        public ViewProviderResult(params IView[] views)
        {
            _views = views;
        }

        public ViewProviderResult(params IPositionedView[] views)
        {
            _views = views;
        }

        public IList<IView> Views
        {
            get => _views ?? (_views = new List<IView>());
        }

    }

}
