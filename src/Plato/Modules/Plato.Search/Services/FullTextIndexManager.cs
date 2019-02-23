using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Services
{
    
    public class FullTextIndexManager : IFullTextIndexManager<FullTextIndex>
    {

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FullTextIndexManager(
            ISchemaBuilder schemaBuilder, 
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }

        public Task<ICommandResult<FullTextIndex>> CreateAsync(FullTextIndex model)
        {
            throw new NotImplementedException();
        }

        public Task<ICommandResult<FullTextIndex>> UpdateAsync(FullTextIndex model)
        {
            throw new NotImplementedException();
        }

        public async Task<ICommandResult<FullTextIndex>> DeleteAsync(FullTextIndex model)
        {
            var result = new CommandResult<FullTextIndex>();

            using (var builder = _schemaBuilder)
            {

                builder.FullTextBuilder.DropIndex(model.TableName, model.ColumnName);

                var errors = (ICollection<string>)await _schemaManager.ExecuteAsync(builder.Statements);
                if (errors.Any())
                {
                    return result.Failed(errors.Select(s => new CommandError(s)).ToArray());
                }

            }

            return result.Success(model);
        }
    }

}
