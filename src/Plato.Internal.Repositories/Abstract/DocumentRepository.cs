using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Abstract;

namespace Plato.Internal.Repositories.Abstract
{
    public class DocumentRepository : IDocumentRepository
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<DictionaryRepository> _logger;

        #endregion

        #region "Constructor"

        public DocumentRepository(
            IDbContext dbContext,
            ILogger<DictionaryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        #endregion

        #region "Implementation"

        public async Task<DocumentEntry> UpdateAsync(DocumentEntry document)
        {
            var id = await InsertUpdateInternal(
                document.Id,
                document.Type,
                document.Value,
                document.CreatedDate,
                document.CreatedUserId,
                document.ModifiedDate,
                document.ModifiedUserId);
            if (id > 0)
            {
                return await GetAsync(id);
            }
            return null;
        }

        public async Task<DocumentEntry> GetAsync(int id)
        {
            return await SelectByIdAsync(id);
        }

        public async Task<DocumentEntry> GetByType(string type)
        {
            return await SelectByTypeAsync(type);
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting document entry with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteDocumentEntryById", id);
            }

            return success > 0 ? true : false;

        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string type,
            string value,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
            int modifiedUserId
        )
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting document with value: {value}"
                    : $"Updating document with id: {id}");
            }

            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateDocumentEntry",
                    id,
                    type.ToEmptyIfNull().TrimToSize(500),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

        }

        async Task<DocumentEntry> SelectByIdAsync(int id)
        {
            DocumentEntry entry = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    entry= await context.ExecuteReaderAsync<DocumentEntry>(
                        CommandType.StoredProcedure,
                        "SelectDocumentEntryById",
                        async reader =>
                        {
                            if (reader != null)
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    entry = new DocumentEntry();
                                    entry.PopulateModel(reader);
                                }
                            }

                            return entry;

                        },
                        id);
                  
                }
            }

            return entry;
        }

        async Task<DocumentEntry> SelectByTypeAsync(string type)
        {
   
            DocumentEntry entry = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    entry = await context.ExecuteReaderAsync<DocumentEntry>(
                        CommandType.StoredProcedure,
                        "SelectDocumentEntryByType",
                        async reader =>
                        {
                            if ((reader != null) && (reader.HasRows))
                            {
                                await reader.ReadAsync();
                                entry = new DocumentEntry();
                                entry.PopulateModel(reader);
                            }

                            return entry;

                        },
                        type);

                }
            }

            return entry;
        }
        
        #endregion
        
    }

}
