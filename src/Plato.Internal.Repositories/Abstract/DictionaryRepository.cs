using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Abstract;
using Microsoft.Extensions.Logging;
using System.Data;
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

        public Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Deleting setting with id: {id}");
            
            throw new NotImplementedException();

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

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting setting with id: {id}");
            
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectSettingById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        var setting = new DictionaryEntry();
                        await reader.ReadAsync();
                        setting.PopulateModel(reader);
                        return setting;
                    }
                }
          
            }

            return null;
            
        }

        public async Task<IEnumerable<DictionaryEntry>> SelectEntries()
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Selecting all settings");

            List<DictionaryEntry> entry = null;
            // database context may not be configured.
            // For example during set-up
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    var reader = await context.ExecuteReaderAsync(
                        CommandType.StoredProcedure,
                        "SelectSettings");
                    if (reader != null)
                    {
                        if (reader.HasRows)
                        {
                            entry = new List<DictionaryEntry>();
                            while (await reader.ReadAsync())
                            {
                                var setting = new DictionaryEntry();
                                setting.PopulateModel(reader);
                                entry.Add(setting);
                            }
                        }
                    }
                }
            }

            return entry;

        }
        

        public async Task<DictionaryEntry> SelectEntryByKey(string key)
        {

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Selecting dictionary entry {key}");

            DictionaryEntry entry = null;
            // database context may not be configured.
            // For example during set-up
            if (_dbContext != null)
            {
                using (var context = _dbContext)
                {
                    var reader = await context.ExecuteReaderAsync(
                        CommandType.StoredProcedure,
                        "SelectSettingByKey",
                        key);
                    if (reader != null)
                    {
                        if (reader.HasRows)
                        {

                            entry = new DictionaryEntry();
                            entry.PopulateModel(reader);

                        }
                    }
                }
            }

            return entry;

        }


        public Task<bool> DeleteByKeyAsync(string key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,   
            string key,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId
            )
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting settings with key: {key}"
                    : $"Updating settings with id: {id}");
            }
              
            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateSetting",
                    id,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId);
            }
            
        }

        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
