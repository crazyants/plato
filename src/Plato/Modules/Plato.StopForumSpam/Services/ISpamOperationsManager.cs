using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.StopForumSpam.Services
{
    public interface ISpamOperationsManager<TOperation> where TOperation : class
    {

        IEnumerable<TOperation> GetSpamOperations();

        Task<IDictionary<string, IEnumerable<TOperation>>> GetCategorizedSpamOperationsAsync();

    }

}
