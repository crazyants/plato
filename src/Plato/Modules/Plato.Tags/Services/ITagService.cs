using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Tags.Services
{
    public interface ITagService<TModel> where TModel : class, ITag
    {
        Task<IPagedResults<TModel>> GetResultsAsync(TagIndexOptions options, PagerOptions pager);
    }


}
