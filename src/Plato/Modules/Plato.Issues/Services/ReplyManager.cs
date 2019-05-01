using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Issues.Services
{

    public class ReplyManager : IPostManager<Comment>
    {
        
        private readonly IEntityReplyManager<Comment> _entityReplyManager;
        
        public ReplyManager(
            IEntityReplyManager<Comment> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
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
