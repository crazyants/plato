using System.Collections.Generic;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamOperationTypeProvider<out TOperation> where TOperation : class, ISpamOperationType
    {
        IEnumerable<TOperation> GetSpamOperationTypes();
        
    }

}
