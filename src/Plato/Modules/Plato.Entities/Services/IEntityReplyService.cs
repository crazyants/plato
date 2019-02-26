using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Entities.ViewModels;

namespace Plato.Entities.Services
{
    public interface IEntityReplyService<TModel> where TModel : class, IEntityReply
    {
        Task<IPagedResults<TModel>> GetRepliesAsync(EntityOptions options, PagerOptions pager);

    }

}
