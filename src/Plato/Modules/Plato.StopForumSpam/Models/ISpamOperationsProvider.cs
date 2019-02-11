using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamOperationsProvider<TOperation> where TOperation : class, ISpamOperation
    {
        IEnumerable<TOperation> GetSpamOperations();


    }

}
