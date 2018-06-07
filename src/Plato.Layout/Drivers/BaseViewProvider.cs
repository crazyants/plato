using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.Drivers
{
    public abstract class BaseViewProvider<TModel> 
    : IViewProvider<TModel>
        where TModel : class
    {
        public abstract Task<IViewResult> BuildDisplayAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewResult> BuildEditAsync(TModel model, IUpdateModel updater);
        
        public abstract Task<IViewResult> BuildUpdateAsync(TModel model, IUpdateModel updater);



        public async Task<IViewResult> Initialize<TViewModel>(string viewName, Func<TViewModel, TViewModel> configure) where TViewModel : class
        {

            var model = configure(default(TViewModel));

            var viewResult = new ViewResult();

            return viewResult;


        }

    }
}
