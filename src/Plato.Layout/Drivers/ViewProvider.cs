using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.Drivers
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> Display(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> Edit(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> Update(TModel model, IUpdateModel updater);

    }

}
