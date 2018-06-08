using System.Collections.Generic;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{

    public class ProviderDisplayContext
    {
        public IGenericView View { get; set; }
    }

    public class ProviderEditContext
    {
        public IGenericView View { get; set; }
    }
    
    public interface IViewProviderResult
    {
        IList<IGenericView> Views { get; set; }
    }

    public class ViewProviderResult : IViewProviderResult
    {
 
        private IList<IGenericView> _views;

        public IList<IGenericView> Views
        {
            get => _views ?? (_views = new List<IGenericView>());
            set => _views = value;
        }

    }
    
    public class CombinedViewProviderResult : IViewProviderResult
    {
        private readonly IList<IViewProviderResult> _results;
        
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

        private IList<IGenericView> _views;

        public IList<IGenericView> Views
        {
            get
            {
                var views = new List<IGenericView>();
                if (_results != null)
                {
                    foreach (var result in _results)
                    {
                        views.AddRange(result.Views);
                    }
                }

                return views;
            }
            set => _views = value;
        }

        
    }
}
