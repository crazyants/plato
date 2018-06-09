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

        public async Task<IViewProviderResult> View<TViewModel>(string viewName,
            Func<TViewModel, Task<TViewModel>> configure) where TViewModel : new()
        {
            
            // Create proxy model 
            var proxyModel = Activator.CreateInstance(typeof(TViewModel));

            // Configure model
            var model = await configure((TViewModel)proxyModel);

            // Return view provider result
            return new ViewProviderResult(
                new PositionedView(viewName, model)
            );

        }

        public CombinedViewProviderResult Views(params IViewProviderResult[] results)
        {
            return new CombinedViewProviderResult(results);
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