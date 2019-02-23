using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Services
{
    
    public class FullTextIndexManager : IFullTextIndexManager<FullTextIndex>
    {
        public Task<ICommandResult<FullTextIndex>> CreateAsync(FullTextIndex model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextIndex>> UpdateAsync(FullTextIndex model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextIndex>> DeleteAsync(FullTextIndex model)
        {
            throw new NotImplementedException();
        }
    }

}
