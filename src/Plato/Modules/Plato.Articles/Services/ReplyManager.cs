using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;

namespace Plato.Articles.Services
{

    public class ReplyManager : IPostManager<Comment>
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyManager<Comment> _entityReplyManager;
        private readonly IEntityReplyStore<Comment> _entityReplyStore;

        public ReplyManager(
            IEntityStore<Article> entityStore,
            IEntityReplyStore<Comment> entityReplyStore,
            IEntityReplyManager<Comment> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public async Task<ICommandResult<Comment>> CreateAsync(Comment model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<ICommandResult<Comment>> UpdateAsync(Comment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<ICommandResult<Comment>> DeleteAsync(Comment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.DeleteAsync(model);
            
        }

        #endregion
        
    }

}
