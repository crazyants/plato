using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Docs.Services
{

    public class ReplyManager : IPostManager<DocComment>
    {

        private readonly IEntityReplyManager<DocComment> _entityReplyManager;
    
        public ReplyManager(
            IEntityReplyManager<DocComment> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
        }

        #region "Implementation"

        public async Task<ICommandResult<DocComment>> CreateAsync(DocComment model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<ICommandResult<DocComment>> UpdateAsync(DocComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<ICommandResult<DocComment>> DeleteAsync(DocComment model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.DeleteAsync(model);
            
        }

        #endregion
        
    }

}
