using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{

    public class EntityRepository : IEntityRepository<Entity>
    {
        
        #region "Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityRepository> _logger;
        private readonly IEntityDataRepository<EntityData> _entityDataRepository;

        #endregion

        #region "Constructor"

        public EntityRepository(
            IDbContext dbContext,
            ILogger<EntityRepository> logger,
            IEntityDataRepository<EntityData> entityDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _entityDataRepository = entityDataRepository;
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
                    "DeleteEntityById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<Entity> InsertUpdateAsync(Entity entity)
        {

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var id = await InsertUpdateInternal(
                entity.Id,
                entity.FeatureId,
                entity.Title,
                entity.TitleNormalized,
                entity.Message,
                entity.Html,
                entity.Abstract,
                entity.IsPublic,
                entity.IsSpam,
                entity.IsPinned,
                entity.IsDeleted,
                entity.IsClosed,
                entity.CreatedUserId,
                entity.CreatedDate,
                entity.ModifiedUserId,
                entity.ModifiedDate,
                entity.Data);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Entity> SelectByIdAsync(int id)
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
                    "SelectEntityById", id);

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
                    "SelectEntitiesPaged",
                    inputParameters
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<T>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new Entity();
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

        private async Task<Entity> BuildEntityFromResultSets(DbDataReader reader)
        {
            Entity entity = null;
            if ((reader != null) && (reader.HasRows))
            {

                entity = new Entity();
                await reader.ReadAsync();
                if (reader.HasRows)
                {
                    entity.PopulateModel(reader);
                }

                // data

                if (await reader.NextResultAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var entityData = new EntityData(reader);
                            entity.Data.Add(entityData);
                        }

                    }
                }
                
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
            int featureId,
            string title,
            string titleNormalized,
            string message,
            string html,
            string messageAbstract,
            bool isPublic,
            bool isSpam,
            bool isPinned,
            bool isDeleted,
            bool isClosed,
            int createdUserId,
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate,
            IEnumerable<EntityData> data)
        {

            var entityId = 0;
            using (var context = _dbContext)
            {

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for entity with title '{title}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for entity with Id {id} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

                entityId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntity",
                    id,
                    featureId,
                    title.ToEmptyIfNull().TrimToSize(255),
                    titleNormalized.ToEmptyIfNull().TrimToSize(255),
                    message.ToEmptyIfNull(),
                    html.ToEmptyIfNull(),
                    messageAbstract.ToEmptyIfNull().TrimToSize(500),
                    isPublic,
                    isSpam,
                    isPinned,
                    isDeleted,
                    isClosed,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull());
            }

            // Add entity data
            if (entityId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.EntityId = entityId;
                        await _entityDataRepository.InsertUpdateAsync(item);
                    }
                }

            }

            return entityId;

        }

        #endregion
    }

}