using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Repositories.Shell
{
 
    public class ShellFeatureRepository : IShellFeatureRepository<ShellFeature>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<ShellFeatureRepository> _logger;
        
        public ShellFeatureRepository(
            IDbContext dbContext,
            ILogger<ShellFeatureRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"
        
        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteShellFeatureById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<ShellFeature> InsertUpdateAsync(ShellFeature feature)
        {

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }
                
            var id = await InsertUpdateInternal(
                feature.Id,
                feature.ModuleId,
                feature.Version,
                feature.Settings);
            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<ShellFeature> SelectByIdAsync(int id)
        {

            ShellFeature feature = null;
            using (var context = _dbContext)
            {
                feature = await context.ExecuteReaderAsync<ShellFeature>(
                    CommandType.StoredProcedure,
                    "SelectShellFeatureById",
                    async reader => await BuildObjectFromResultSets(reader),
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return feature;

        }
        
        public async Task<IPagedResults<ShellFeature>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<ShellFeature> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<IPagedResults<ShellFeature>>(
                    CommandType.StoredProcedure,
                    "SelectShellFeaturesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<ShellFeature>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new ShellFeature();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                            return output;
                        }

                        return null;
                    },
                    dbParams
                );
                
            }

            return results;

        }

        public async Task<IEnumerable<ShellFeature>> SelectFeatures()
        {

            IList<ShellFeature> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectShellFeatures",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            data = new List<ShellFeature>();
                            while (await reader.ReadAsync())
                            {
                                var item = new ShellFeature();
                                item.PopulateModel(reader);
                                data.Add(item);
                            }
                            return data;
                        }

                        return null;

                    });

            }

            return data;

        }

        #endregion

        #region "Private Methods"

        async Task<ShellFeature> BuildObjectFromResultSets(DbDataReader reader)
        {
            ShellFeature feature = null;
            if ((reader != null) && (reader.HasRows))
            {
                feature = new ShellFeature();
                await reader.ReadAsync();
                 feature.PopulateModel(reader);
            }
            return feature;
        }

        async Task<int> InsertUpdateInternal(
            int id,
            string moduleId,
            string version,
            string settings)
        {

            // We always need a ModuleId
            if (String.IsNullOrEmpty(moduleId))
            {
                throw new ArgumentNullException(nameof(moduleId));
            }

            if (String.IsNullOrEmpty(version))
            {
                version = "1.0.0";
            }
            
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateShellFeature",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ModuleId", DbType.String, 255, moduleId),
                        new DbParam("Version", DbType.String, 10, version),
                        new DbParam("Settings", DbType.String, settings),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }
        }

        #endregion

    }

}
