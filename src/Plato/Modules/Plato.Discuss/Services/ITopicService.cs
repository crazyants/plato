using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Services
{

    public interface ITopicService
    {

        Task<IPagedResults<Topic>> GetTopicsAsync(TopicIndexOptions options, PagerOptions pager);

    }

}
