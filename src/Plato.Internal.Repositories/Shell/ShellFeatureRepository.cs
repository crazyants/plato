using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Repositories.Shell
{
    public interface IShellFeatureRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectFeatures();
    }

    public class ShellFeatureRepository : IShellFeatureRepository<ShellFeature>
    {

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<ShellFeatureRepository> _logger;

        #endregion

        #region "Constructor"

        public ShellFeatureRepository(
            IDbContext dbContext,
            ILogger<ShellFeatureRepository> logger)
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
                _logger.LogInformation($"Deleting entity with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteShellFeatureById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<ShellFeature> InsertUpdateAsync(ShellFeature feature)
        {

            if (feature == null)
                throw new ArgumentNullException(nameof(feature));

            var id = await InsertUpdateInternal(
                feature.Id,
                feature.ModuleId,
                feature.Version);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<ShellFeature> SelectByIdAsync(int id)
        {
            using (var context = _dbContext)
            {
                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            $"Selecting feature for Id {id} failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectShellFeatureById", id);

                return await BuildObjectFromResultSets(reader);
            }
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] inputParameters) where T : class
        {
            PagedResults<T> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectShellFeaturesPaged",
                    inputParameters
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<T>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new ShellFeature();
                        entity.PopulateModel(reader);
                        output.Data.Add((T)Convert.ChangeType(entity, typeof(T)));
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }
                }
            }

            return output;
        }


        public async Task<IPagedResults<ShellFeature>> SelectAsync(params object[] inputParams)
        {
            PagedResults<ShellFeature> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectShellFeaturesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<ShellFeature>();
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
                }
            }

            return output;
        }

        public async Task<IEnumerable<ShellFeature>> SelectFeatures()
        {

            var data = new List<ShellFeature>();
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectShellFeatures");
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var userData = new ShellFeature();
                            userData.PopulateModel(reader);
                            data.Add(userData);
                        }
                    }
                }
            }

            return data;

        }

        #endregion

        #region "Private Methods"

        private async Task<ShellFeature> BuildObjectFromResultSets(DbDataReader reader)
        {
            ShellFeature feature = null;
            if ((reader != null) && (reader.HasRows))
            {
                feature = new ShellFeature();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    feature.PopulateModel(reader);
                }
            }
            return feature;
        }

        private async Task<int> InsertUpdateInternal(
            int id,
            string moduleId,
            string version)
        {
            using (var context = _dbContext)
            {

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for shell feature '{moduleId}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for shell feature with Id {moduleId} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateShellFeature",
                    id,
                    moduleId.ToEmptyIfNull(),
                    version.ToEmptyIfNull());

            }
        }

        #endregion

    }

}
