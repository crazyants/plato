using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperationManager<TOperation> where TOperation : class
    {

        IEnumerable<TOperation> GetSpamOperations();

        Task<IDictionary<string, IEnumerable<TOperation>>> GetCategorizedSpamOperationsAsync();

    }

}
