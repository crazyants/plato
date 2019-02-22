using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Data.Schemas.Abstractions;

namespace Plato.Internal.Data.Schemas
{
    
    public class SchemaManager : ISchemaManager
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<SchemaBuilder> _logger;

        public SchemaManager(
            IDbContext dbContext, 
            ILogger<SchemaBuilder> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> ExecuteAsync(IEnumerable<string> statements)
        {

            if (statements == null)
            {
                throw new ArgumentNullException(nameof(statements));
            }

            var errors = new List<string>();
            foreach (var statement in statements)
            {
                try
                {

                    // Log statements to execute
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation($"Attempting to execute SQL statement:- {statement}");

                    }

                    using (var context = _dbContext)
                    {
                        await context.ExecuteNonQueryAsync<int>(CommandType.Text, statement);
                    }
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, e.Message);
                    }
                    errors.Add(e.Message);
                }

            }

            return errors;
            
        }

    }

}
