using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout.ViewProviders
{
    
    public abstract class BaseViewProvider<TModel>
        : IViewProvider<TModel> where TModel : class
    {
        
        #region "Abstract Medthods"

        public abstract Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        public abstract Task<IViewProviderResult> BuildIndexAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        public abstract Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

        #endregion

        #region "Helper Methods"
        
        public LayoutViewModel Views(params IView[] views)
        {
            // TODO: // Implement a context object allowing uss to pass the service provider along
            return new LayoutViewModel(
                new ViewProviderResult(views)
            );
        }

        public IPositionedView View<TViewModel>(
            string viewName,
            Func<TViewModel, TViewModel> configure) where TViewModel : new()
        {
       
            // Create proxy model 
            var proxy = Activator.CreateInstance(typeof(TViewModel));

            // Configure model
            var model = configure((TViewModel) proxy);

            // Return a view we can optionally position
            return new PositionedView(viewName, model);

        }
  
        
        #endregion

    }
}