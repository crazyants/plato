using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Services
{
    
    public class FullTextCatalogManager : IFullTextCatalogManager<FullTextCatalog>
    {
        public Task<ICommandResult<FullTextCatalog>> CreateAsync(FullTextCatalog model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextCatalog>> UpdateAsync(FullTextCatalog model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextCatalog>> DeleteAsync(FullTextCatalog model)
        {
            throw new NotImplementedException();
        }
    }
}
