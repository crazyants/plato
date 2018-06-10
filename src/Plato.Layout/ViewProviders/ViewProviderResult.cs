using System.Collections.Generic;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{
    public interface IViewProviderResult
    {
        IEnumerable<IView> Views { get; }
    }

    public class ViewProviderResult : IViewProviderResult
    {

        private IEnumerable<IView> _views;

        public ViewProviderResult(params IView[] views)
        {
            _views = views;
        }

        public ViewProviderResult(params IPositionedView[] views)
        {
            _views = views;
        }

        public IEnumerable<IView> Views
        {
            get => _views ?? (_views = new List<IView>());
        }

    }

}
