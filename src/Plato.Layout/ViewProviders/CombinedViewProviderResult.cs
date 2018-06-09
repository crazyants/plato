using System.Collections.Generic;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{

    public class CombinedViewProviderResult : IViewProviderResult
    {
        private readonly IList<IViewProviderResult> _results;
        private IList<IView> _views;

        public CombinedViewProviderResult(params IViewProviderResult[] results)
        {
            _results = results;
        }

        public CombinedViewProviderResult(IList<IViewProviderResult> results)
        {
            _results = results;
        }

        public IList<IViewProviderResult> GetResults()
        {
            return _results;
        }

        public IList<IView> Views
        {
            get
            {
                var views = new List<IView>();
                if (_results != null)
                {
                    foreach (var result in _results)
                    {
                        views.AddRange(result.Views);
                    }
                }

                return views;
            }
           
        }


    }
}