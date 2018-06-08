using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;
using Plato.Layout.Views;

namespace Plato.Layout.ViewProviders
{
    public abstract class BaseViewProvider<TModel> 
    : IViewProvider<TModel>
        where TModel : class
    {
        public abstract Task<IViewProviderResult> DisplayAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> EditAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> UpdateAsync(TModel model, IUpdateModel updater);

        public async Task<IViewProviderResult> View<TViewModel>(string viewName,
            Func<TViewModel, Task<TViewModel>> configure) where TViewModel : new()
        {

            // Create proxy model 
            var activator = Activator.CreateInstance(typeof(TViewModel));

            // Configure model
            var model = await configure((TViewModel) activator);

            // Return view provider result
            return new ViewProviderResult()
            {
                Views =
                {
                    new View(viewName, model)
                }
            };

        }

        public CombinedViewProviderResult Combine(params IViewProviderResult[] results)
        {
            return new CombinedViewProviderResult(results);
        }


        //private static IShape CreateShape(Type baseType)
        //{
        //    // Don't generate a proxy for shape types
        //    if (typeof(IShape).IsAssignableFrom(baseType))
        //    {
        //        var shape = Activator.CreateInstance(baseType) as IShape;
        //        return shape;
        //    }
        //    else
        //    {
        //        var options = new ProxyGenerationOptions();
        //        options.AddMixinInstance(new ShapeViewModel());
        //        return (IShape)ProxyGenerator.CreateClassProxy(baseType, options);
        //    }
        //}

    }
}
