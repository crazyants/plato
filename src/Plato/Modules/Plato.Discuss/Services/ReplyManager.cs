using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;

namespace Plato.Discuss.Services
{

    public class ReplyManager : IPostManager<Reply>
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyManager<Reply> _entityReplyManager;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        public ReplyManager(
            IEntityStore<Topic> entityStore,
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityReplyManager<Reply> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public async Task<IActivityResult<Reply>> CreateAsync(Reply model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<IActivityResult<Reply>> UpdateAsync(Reply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<IActivityResult<Reply>> DeleteAsync(Reply model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {

            };

            return await _entityReplyManager.DeleteAsync(model.Id);
            
        }

        #endregion
        
    }

}
