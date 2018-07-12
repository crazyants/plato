using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{

    public class EntityRepository<TModel> : IEntityRepository<TModel> where TModel : class, IEntity
    {
     
        #region "Constructor"
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityRepository<TModel>> _logger;
        private readonly IEntityDataRepository<IEntityData> _entityDataRepository;

        public EntityRepository(
            IDbContext dbContext,
            ILogger<EntityRepository<TModel>> logger,
            IEntityDataRepository<IEntityData> entityDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _entityDataRepository = entityDataRepository;
        }

        #endregion
        
        #region "Implementation"

        public async Task<TModel> InsertUpdateAsync(TModel entity)
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
                entity.IsPrivate,
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

        public async Task<TModel> SelectByIdAsync(int id)
        {

            TModel entity = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityById", id);
                entity = await BuildEntityFromResultSets(reader);
            }

            return entity;
        }
        
        public async Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams)
        {
            PagedResults<TModel> output = null;
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
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<TModel>();
                    while (await reader.ReadAsync())
                    {
                        var entity = (TModel) Activator.CreateInstance(typeof(TModel));
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

        #endregion

        #region "Private Methods"

        async Task<TModel> BuildEntityFromResultSets(DbDataReader reader)
        {
            TModel entity = null;
            if ((reader != null) && (reader.HasRows))
            {

                entity = (TModel)Activator.CreateInstance(typeof(TModel));

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
                        var data = new List<EntityData>();
                        while (await reader.ReadAsync())
                        {
                            var entityData = new EntityData(reader);
                            data.Add(entityData);
                        }
                        entity.Data = data;
                    }

                }

            }

            return entity;
        }

        async Task<int> InsertUpdateInternal(
            int id,
            int featureId,
            string title,
            string titleNormalized,
            string message,
            string html,
            string messageAbstract,
            bool isPrivate,
            bool isSpam,
            bool isPinned,
            bool isDeleted,
            bool isClosed,
            int createdUserId,
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate,
            IEnumerable<IEntityData> data)
        {

            var entityId = 0;
            using (var context = _dbContext)
            {

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for entity with title '{title}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for entity with Id {id} failed with the following error {args.Exception.Message}");
                    }

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
                    isPrivate,
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