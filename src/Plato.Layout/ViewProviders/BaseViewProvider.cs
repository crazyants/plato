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

        public abstract Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        public abstract Task<IViewProviderResult> UpdateAsync(TModel model, IUpdateModel updater);

        #endregion

        #region "Helper Methods"

        public IPositionedView View<TViewModel>(
            string viewName,
            Func<TViewModel, TViewModel> configure) where TViewModel : new()
        {
            
            // Create proxy model 
            var proxyModel = Activator.CreateInstance(typeof(TViewModel));

            // Configure model
            var model = configure((TViewModel)proxyModel);

            // Return a view we can optionally position
            return new PositionedView(viewName, model);

        }

        public CombinedViewProviderResult Views(params IView[] views)
        {
            return new CombinedViewProviderResult(
                new ViewProviderResult(views)
            );

        }

        //private static IView CreateModel(Type baseType)
        //{
        //    // Don't generate a proxy for shape types
        //    if (typeof(IView).IsAssignableFrom(baseType))
        //    {
        //        var shape = Activator.CreateInstance(baseType) as IView;
        //        return shape;
        //    }
        //    else
        //    {
        //        //var options = new ProxyGenerationOptions();
        //        //options.AddMixinInstance(new ShapeViewModel());
        //        //return (IShape)ProxyGenerator.CreateClassProxy(baseType, options);
        //    }

        //    return null;
        //}

        #endregion

    }
}