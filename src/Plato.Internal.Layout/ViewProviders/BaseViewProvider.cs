using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Layout.EmbeddedViews;
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

        public virtual Task<bool> ValidateModelAsync(TModel model, IUpdateModel updater)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region "Helper Methods"

        public IViewProviderResult Views(params IView[] views)
        {
            // TODO: // Implement a context object allowing uss to pass the service provider along
            return new LayoutViewModel(
                new ViewProviderResult(views)
            );
        }

        public IPositionedView View<TViewModel>(string viewName, Func<TViewModel, TViewModel> configure) where TViewModel : class
        {

            // Create proxy model 
            var proxy = ActivateInstanceOf<TViewModel>.Instance();
        
            // Configure model
            var model = configure((TViewModel) proxy);

            // Return a view we can optionally position
            return new PositionedView(viewName, model);

        }
        
        public IPositionedView View(string viewName, dynamic arguments)
        {
            // Used to invoke view components
            return new PositionedView(viewName, arguments);
        }
        
        public IPositionedView View(IEmbeddedView enbeddedView)
        {
            // Return a view we can optionally position
            return new PositionedView(enbeddedView);
        }

        #endregion

    }

}