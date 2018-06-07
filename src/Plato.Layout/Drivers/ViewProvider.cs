using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.Drivers
{

    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewResult> BuildEditAsync(TModel model, IUpdateModel updater);

        Task<IViewResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

    }

}
