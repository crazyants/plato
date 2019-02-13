using System.Collections.Generic;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.Services
{

    public interface ISpamOperationProvider<out TOperation> where TOperation : class, ISpamOperation
    {
        IEnumerable<TOperation> GetSpamOperations();
        
    }

}
