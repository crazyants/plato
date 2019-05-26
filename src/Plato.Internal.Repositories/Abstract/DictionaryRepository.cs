using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Abstract;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories.Abstract
{
    public class DictionaryRepository : IDictionaryRepository<DictionaryEntry>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<DictionaryRepository> _logger;
     
        #endregion

        #region "Constructor"

        public DictionaryRepository(
            IDbContext dbContext,
            ILogger<DictionaryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"
        
        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting dictionary entry with id: {id}");
            }
                
            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteDictionaryEntryById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<DictionaryEntry> InsertUpdateAsync(DictionaryEntry dictionaryEntry)
        {
            var id = await InsertUpdateInternal(
                dictionaryEntry.Id,    
                dictionaryEntry.Key,
                dictionaryEntry.Value,
                dictionaryEntry.CreatedDate,
                dictionaryEntry.CreatedUserId,
                dictionaryEntry.ModifiedDate,
                dictionaryEntry.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public async Task<DictionaryEntry> SelectByIdAsync(int id)
        {
     
            DictionaryEntry output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectDictionaryEntryById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new DictionaryEntry();
                            await reader.ReadAsync();
                            output.PopulateModel(reader);
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return output;
            
        }

        public async Task<IEnumerable<DictionaryEntry>> SelectEntries()
        {
            IList<DictionaryEntry> output = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    output = await context.ExecuteReaderAsync2<IList<DictionaryEntry>>(
                        CommandType.StoredProcedure,
                        "SelectDictionaryEntries",
                        async reader =>
                        {
                            if ((reader != null) && (reader.HasRows))
                            {
                                output = new List<DictionaryEntry>();
                                while (await reader.ReadAsync())
                                {
                                    var entry = new DictionaryEntry();
                                    entry.PopulateModel(reader);
                                    output.Add(entry);
                                }
                            }

                            return output;

                        });
                }
            }

            return output;

        }
        
        public async Task<DictionaryEntry> SelectEntryByKey(string key)
        {
            DictionaryEntry entry = null;
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    entry = await context.ExecuteReaderAsync2(
                        CommandType.StoredProcedure,
                        "SelectDictionaryEntryByKey",
                        async reader =>
                        {
                            if ((reader != null) && (reader.HasRows))
                            {
                                entry = new DictionaryEntry();
                                await reader.ReadAsync();
                                entry.PopulateModel(reader);
                            }

                            return entry;
                        }, new[]
                        {
                            new DbParam("Key", DbType.Int32, key)
                        });

                }
            }

            return entry;

        }
        
        public Task<bool> DeleteByKeyAsync(string key)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<IPagedResults<DictionaryEntry>> SelectAsync(DbParam[] dbParams)
        {
            // TODO
            throw new NotImplementedException();
        }


        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,   
            string key,
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
                    ? $"Inserting dictionary entry with key: {key}"
                    : $"Updating dictionary entry with id: {id}");
            }
              
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateDictionaryEntry",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("Key", DbType.String, 255, key),
                        new DbParam("Value", DbType.String, value),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }
            
        }
        
        #endregion

    }
}
