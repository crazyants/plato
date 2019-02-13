using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperationTypeManager<TOperation> where TOperation : class
    {

        IEnumerable<TOperation> GetSpamOperations();

        Task<IDictionary<string, IEnumerable<TOperation>>> GetCategorizedSpamOperationsAsync();

    }

}
