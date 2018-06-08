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



        public async Task<IViewProviderResult> Initialize<TViewModel>(string viewName, Func<TViewModel, TViewModel> configure) where TViewModel : class
        {

            var model = configure(default(TViewModel));

            var viewResult = new ViewProviderResult();
            viewResult.Views.Add(new GenericView(viewName, model));
          
            return viewResult;


        }

    }
}
