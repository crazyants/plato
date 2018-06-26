using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Repositories.Features
{
    public interface IFeatureRepository<T> : IRepository<T> where T : class
    {
    }

    public class FeatureRepository : IFeatureRepository<ShellFeature>
    {

        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<FeatureRepository> _logger;

        #endregion

        #region "Constructor"

        public FeatureRepository(
            IDbContext dbContext,
            ILogger<FeatureRepository> logger)
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
                    "DeleteFeatureById", id);
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
                //// secerts

                //if (user.Secret == null)
                //    user.Secret = new UserSecret();
                //if ((user.Id == 0) || (user.Secret.UserId == 0))
                //    user.Secret.UserId = id;
                //await _userSecretRepository.InsertUpdateAsync(user.Secret);

                //// detail

                //if (user.Detail == null)
                //    user.Detail = new UserDetail();
                //if ((user.Id == 0) || (user.Detail.UserId == 0))
                //    user.Detail.UserId = id;
                //await _userDetailRepository.InsertUpdateAsync(user.Detail);

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
                            $"SelectUser for Id {id} failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFeatureById", id);

                return await BuildEntityFromResultSets(reader);
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
                    "SelectFeaturesPaged",
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

        #endregion

        #region "Private Methods"

        private async Task<ShellFeature> BuildEntityFromResultSets(DbDataReader reader)
        {
            ShellFeature entity = null;
            if ((reader != null) && (reader.HasRows))
            {
                // user

                entity = new ShellFeature();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    //entity.PopulateModel(reader);
                }

                //// data

                //if (await reader.NextResultAsync())
                //{
                //    if (reader.HasRows)
                //    {
                //        await reader.ReadAsync();
                //        user.Detail = new UserDetail(reader);
                //    }
                //}

                //// roles

                //if (await reader.NextResultAsync())
                //{
                //    if (reader.HasRows)
                //    {
                //        while (await reader.ReadAsync())
                //        {
                //            user.UserRoles.Add(new Role(reader));
                //        }
                //    }
                //}

            }
            return entity;
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
                                ? $"Insert for feature '{moduleId}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for feature with Id {moduleId} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateFeature",
                    id,
                    moduleId.ToEmptyIfNull(),
                    version.ToEmptyIfNull());

            }
        }

        #endregion
    }

}
