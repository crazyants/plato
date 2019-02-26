using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Services
{
    public interface IReplyService
    {
        Task<IPagedResults<Reply>> GetRepliesAsync(EntityOptions options, PagerOptions pager);

    }

}
