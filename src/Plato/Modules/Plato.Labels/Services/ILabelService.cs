using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.ViewModels;

namespace Plato.Labels.Services
{
    public interface ILabelService<TModel> where TModel : class, ILabel
    {
        Task<IPagedResults<TModel>> GetResultsAsync(LabelIndexOptions options, PagerOptions pager);
    }

}
