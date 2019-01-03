using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Labels.Services
{
    public interface ILabelService
    {
        Task<IPagedResults<Label>> GetLabelsAsunc(LabelIndexOptions options, PagerOptions pager);
    }

}
