using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Labels.Services
{

    public interface IEntityHistoryManager<TLabel> : ICommandManager<TLabel> where TLabel : class
    {
   
   

    }

}
