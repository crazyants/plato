using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;

namespace Plato.Articles.Services
{

    public class ReplyManager : IPostManager<ArticleComment>
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyManager<ArticleComment> _entityReplyManager;
        private readonly IEntityReplyStore<ArticleComment> _entityReplyStore;

        public ReplyManager(
            IEntityStore<Article> entityStore,
            IEntityReplyStore<ArticleComment> entityReplyStore,
            IEntityReplyManager<ArticleComment> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        #region "Implementation"

        public async Task<ICommandResult<ArticleComment>> CreateAsync(ArticleComment model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<ICommandResult<ArticleComment>> UpdateAsync(ArticleComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<ICommandResult<ArticleComment>> DeleteAsync(ArticleComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.DeleteAsync(model);
            
        }

        #endregion
        
    }

}
