using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;

namespace Plato.Questions.Services
{

    public class AnswerManager : IPostManager<Answer>
    {
        
        private readonly IEntityReplyManager<Answer> _entityReplyManager;
        
        public AnswerManager(
            IEntityReplyManager<Answer> entityReplyManager)
        {
            _entityReplyManager = entityReplyManager;
        }

        #region "Implementation"

        public async Task<ICommandResult<Answer>> CreateAsync(Answer model)
        {

            _entityReplyManager.Created += (sender, args) =>
            {
            };
            return await _entityReplyManager.CreateAsync(model);
        
        }
        
        public async Task<ICommandResult<Answer>> UpdateAsync(Answer model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.UpdateAsync(model);
         
        }
        
        public async Task<ICommandResult<Answer>> DeleteAsync(Answer model)
        {

            _entityReplyManager.Updated += (sender, args) =>
            {
            };
            return await _entityReplyManager.DeleteAsync(model);
            
        }

        #endregion
        
    }

}
    }

}
