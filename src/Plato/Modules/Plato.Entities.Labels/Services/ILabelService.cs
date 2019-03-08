using System.Threading.Tasks;
using Plato.Entities.Labels.Models;
using Plato.Entities.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Labels.Services
{
    public interface ILabelService<TModel> where TModel : class, ILabel
    {
        Task<IPagedResults<TModel>> GetResultsAsync(LabelIndexOptions options, PagerOptions pager);
    }

}
