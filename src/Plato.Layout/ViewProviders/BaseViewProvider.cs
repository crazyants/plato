using System;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{
    
    public abstract class BaseViewProvider<TModel>
        : IViewProvider<TModel> where TModel : class
    {
        
        #region "Abstract Medthods"

        public abstract Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        public abstract Task<IViewProviderResult> BuildLayoutAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        public abstract Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

        #endregion

        #region "Helper Methods"

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
        
        public LayoutViewModel Layout(params IView[] views)
        {
            // Return the layout view provider result and build the model
            return new LayoutViewModel(
                new ViewProviderResult(views)
            );
        }

        public CombinedViewProviderResult Views(params IView[] views)
        {
            return new CombinedViewProviderResult(
                new ViewProviderResult(views)
            );
        }
        
        #endregion

    }
}