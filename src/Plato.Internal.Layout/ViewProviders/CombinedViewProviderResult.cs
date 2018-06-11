using System.Collections.Generic;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout.ViewProviders
{

    public class CombinedViewProviderResult : IViewProviderResult
    {
        private readonly IEnumerable<IViewProviderResult> _results;
   
        public CombinedViewProviderResult(params IViewProviderResult[] results)
        {
            _results = results;
        }

        public CombinedViewProviderResult(IList<IViewProviderResult> results)
        {
            _results = results;
        }

        public IEnumerable<IViewProviderResult> GetResults()
        {
            return _results;
        }

        public IEnumerable<IView> Views
        {
            get
            {
                var views = new List<IView>();
                if (_results != null)
                {
                    foreach (var result in _results)
                    {
                        if (result.Views != null)
                        {
                            views.AddRange(result.Views);
                        }
                    }
                }

                return views;
            }
           
        }


    }
}