using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{
    public abstract class BaseViewProvider<TModel> 
    : IViewProvider<TModel>
        where TModel : class
    {
        public abstract Task<IViewProviderResult> Display(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> Edit(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewProviderResult> Update(TModel model, IUpdateModel updater);
        
        public async Task<IViewProviderResult> Initialize<TViewModel>(string viewName, Func<TViewModel, TViewModel> configure) where TViewModel : new()
        {
          
            var activator = Activator.CreateInstance(typeof(TViewModel));
            var model = configure((TViewModel)activator);

            var viewResult = new ViewProviderResult()
            {
                Views =
                {
                    new GenericView(viewName, model)
                }
            };
           
          
            return viewResult;

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
