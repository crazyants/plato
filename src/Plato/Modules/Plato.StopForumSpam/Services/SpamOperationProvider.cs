using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface ISpamOperationProvider<TModel> where TModel : class
    {
        Task<ICommandResult<TModel>> ExecuteAsync(ISpamOperationContext<TModel> context);
    }
    
}
