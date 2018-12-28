using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Services
{
    
    public class ReplyService : IReplyService
    {

        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        
        public ReplyService(IEntityReplyStore<Reply> entityReplyStore)
        {
            _entityReplyStore = entityReplyStore;
        }

        public async Task<IPagedResults<Reply>> GetRepliesAsync(
            TopicOptions options,
            PagerOptions pager)
        {

            return  await _entityReplyStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(options.Params.EntityId);
                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
                })
                .OrderBy(options.Sort, options.Order)
                .ToList();

        }

    }

}
