using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Plato.Internal.Layout.ModelBinding
{
    public interface IUpdateModel
    {

        Task<bool> TryUpdateModelAsync<TModel>(TModel model) where TModel : class;

        bool TryValidateModel(object model);

        bool TryValidateModel(object model, string prefix);

        ModelStateDictionary ModelState { get; }

    }
}
