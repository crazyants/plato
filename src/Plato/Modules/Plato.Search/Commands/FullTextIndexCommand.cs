using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Search.Stores;

namespace Plato.Search.Commands
{
    
    public class FullTextIndexCommand : IFullTextIndexCommand<SchemaFullTextIndex>
    {
        
        private readonly ICacheManager _cacheManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FullTextIndexCommand(
            ISchemaBuilder schemaBuilder, 
            ISchemaManager schemaManager, 
            ICacheManager cacheManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
            _cacheManager = cacheManager;
        }

        public async Task<ICommandResult<SchemaFullTextIndex>> CreateAsync(SchemaFullTextIndex model)
        {

            // Create result
            var result = new CommandResult<SchemaFullTextIndex>();

            // Drop index
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.CreateIndex(model);

                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            // Expire store
            _cacheManager.CancelTokens(typeof(FullTextIndexStore));

            // Return result
            return result.Success(model);

        }

        public async Task<ICommandResult<SchemaFullTextIndex>> UpdateAsync(SchemaFullTextIndex model)
        {

            // Create result
            var result = new CommandResult<SchemaFullTextIndex>();

            // Drop index
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.AlterIndex(model);

                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            // Expire store
            _cacheManager.CancelTokens(typeof(FullTextIndexStore));

            // Return result
            return result.Success(model);

        }

        public async Task<ICommandResult<SchemaFullTextIndex>> DeleteAsync(SchemaFullTextIndex model)
        {

            // Create result
            var result = new CommandResult<SchemaFullTextIndex>();

            // Drop index
            using (var builder = _schemaBuilder)
            {
                if (model.ColumnNames != null)
                {
                    foreach (var columnName in model.ColumnNames)
                    {
                        builder.FullTextBuilder.DropIndex(model.TableName, columnName);
                    }
                }
                else
                {
                    builder.FullTextBuilder.DropIndexes(model.TableName);
                }
          
                
                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            // Expire index store
            _cacheManager.CancelTokens(typeof(FullTextIndexStore));

            // Return result
            return result.Success(model);

        }

    }

}
