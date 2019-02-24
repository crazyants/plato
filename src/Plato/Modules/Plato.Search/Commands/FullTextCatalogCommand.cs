using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Search.Stores;

namespace Plato.Search.Commands
{
    
    public class FullTextCatalogCommand : IFullTextCatalogCommand<SchemaFullTextCatalog>
    {

        private readonly ICacheManager _cacheManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FullTextCatalogCommand(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager,
            ICacheManager cacheManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
            _cacheManager = cacheManager;
        }

        public async Task<ICommandResult<SchemaFullTextCatalog>> CreateAsync(SchemaFullTextCatalog model)
        {

            // Create result
            var result = new CommandResult<SchemaFullTextCatalog>();

            // Drop catalog
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.CreateCatalog(model.Name);

                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            // Expire catalog store
            _cacheManager.CancelTokens(typeof(FullTextCatalogStore));

            // Return result
            return result.Success(model);

        }

        public async Task<ICommandResult<SchemaFullTextCatalog>> UpdateAsync(SchemaFullTextCatalog model)
        {
            // Create result
            var result = new CommandResult<SchemaFullTextCatalog>();

            // Drop catalog
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.RebuildCatalog(model.Name);

                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            // Expire catalog store
            _cacheManager.CancelTokens(typeof(FullTextCatalogStore));

            // Return result
            return result.Success(model);

        }

        public async Task<ICommandResult<SchemaFullTextCatalog>> DeleteAsync(SchemaFullTextCatalog model)
        {

            // Create result
            var result = new CommandResult<SchemaFullTextCatalog>();

            // Drop catalog
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.DropCatalog(model.Name);

                var errors = (ICollection<string>) await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }
                
            }

            // Expire catalog store
            _cacheManager.CancelTokens(typeof(FullTextCatalogStore));

            // Return result
            return result.Success(model);

        }

    }

}
