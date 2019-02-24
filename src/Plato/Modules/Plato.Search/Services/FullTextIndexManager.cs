using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Search.Models;
using Plato.Search.Stores;

namespace Plato.Search.Services
{
    
    public class FullTextIndexManager : IFullTextIndexManager<FullTextIndex>
    {

        private readonly ICacheManager _cacheManager;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FullTextIndexManager(
            ISchemaBuilder schemaBuilder, 
            ISchemaManager schemaManager, 
            ICacheManager cacheManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
            _cacheManager = cacheManager;
        }

        public Task<ICommandResult<FullTextIndex>> CreateAsync(FullTextIndex model)
        {

            _cacheManager.CancelTokens(typeof(FullTextIndexStore));

            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextIndex>> UpdateAsync(FullTextIndex model)
        {

            _cacheManager.CancelTokens(typeof(FullTextIndexStore));


            throw new NotImplementedException();
        }

        public async Task<ICommandResult<FullTextIndex>> DeleteAsync(FullTextIndex model)
        {

            // Create result
            var result = new CommandResult<FullTextIndex>();

            // Drop index
            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.DropIndex(model.TableName, model.ColumnName);

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
    }

}
