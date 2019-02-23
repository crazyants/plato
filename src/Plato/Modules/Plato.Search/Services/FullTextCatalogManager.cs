using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Services
{
    
    public class FullTextCatalogManager : IFullTextCatalogManager<FullTextCatalog>
    {

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FullTextCatalogManager(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        public Task<ICommandResult<FullTextCatalog>> CreateAsync(FullTextCatalog model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextCatalog>> UpdateAsync(FullTextCatalog model)
        {
            throw new NotImplementedException();
        }

        public async Task<ICommandResult<FullTextCatalog>> DeleteAsync(FullTextCatalog model)
        {

            var result = new CommandResult<FullTextCatalog>();

            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.DropCatalog(model.Name);

                var errors = (ICollection<string>) await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }
                
            }

            return result.Success(model);

        }

    }

}
