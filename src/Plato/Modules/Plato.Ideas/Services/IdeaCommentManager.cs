using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Ideas.Services
{

    public class IdeaCommentManager : IPostManager<IdeaComment>
    {
        
        private readonly IEntityReplyManager<IdeaComment> _entityReplyManager;
        
        public IdeaCommentManager(
            IEntityReplyManager<IdeaComment> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
        }

        #region "Implementation"

        public async Task<ICommandResult<IdeaComment>> CreateAsync(IdeaComment model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<ICommandResult<IdeaComment>> UpdateAsync(IdeaComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<ICommandResult<IdeaComment>> DeleteAsync(IdeaComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.DeleteAsync(model);
            
        }

        #endregion
        
    }

}
   