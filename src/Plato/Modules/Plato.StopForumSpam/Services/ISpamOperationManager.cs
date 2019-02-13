using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperationManager<TModel> where TModel : class
    {

        Task<IEnumerable<ICommandResult<TModel>>> ExecuteAsync(ISpamOperationType operationType, TModel model);

    }

}
