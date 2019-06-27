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
            {
                throw new ArgumentNullException(nameof(entity));
            }
                
            var id = await InsertUpdateInternal(
                entity.Id,
                entity.ParentId,
                entity.FeatureId,
                entity.CategoryId,
                entity.Title,
                entity.Alias,
                entity.Message,
                entity.Html,
                entity.Abstract,
                entity.Urls,
                entity.IsHidden,
                entity.IsPrivate,
                entity.IsSpam,
                entity.IsPinned,
                entity.IsDeleted,
                entity.IsLocked,
                entity.IsClosed,
                entity.TotalViews,
                entity.TotalReplies,
                entity.TotalAnswers,
                entity.TotalParticipants,
                entity.TotalReactions,
                entity.TotalFollows,
                entity.TotalReports,
                entity.TotalStars,
                entity.TotalRatings,
                entity.SummedRating,
                entity.MeanRating,
                entity.TotalLinks,
                entity.TotalImages,
                entity.DailyViews,
                entity.DailyReplies,
                entity.DailyAnswers,
                entity.DailyReactions,
                entity.DailyFollows,
                entity.DailyReports,
                entity.DailyStars,
                entity.DailyRatings,
                entity.SortOrder,
                entity.IpV4Address,
                entity.IpV6Address,
                entity.CreatedUserId,
                entity.CreatedDate,
                entity.EditedUserId,
                entity.EditedDate,
                entity.ModifiedUserId,
                entity.ModifiedDate,
                entity.LastReplyId,
                entity.LastReplyUserId,
                entity.LastReplyDate,
                entity.Data);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<TModel> SelectByIdAsync(int id)
        {

            TModel entity = null;
            using (var context = _dbContext)
            {
                entity = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectEntityById",
                    async reader => await BuildEntityFromResultSets(reader),
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id)

                    });
            }

            return entity;

        }
        
        public async Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<TModel> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<IPagedResults<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectEntitiesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = ActivateInstanceOf<TModel>.Instance();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                            return output;
                        }

                        return null;

                    },
                    dbParams);
               
            }

            return results;
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
                    "DeleteEntityById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                    });
            }

            return success > 0 ? true : false;

        }

        #endregion

        #region "Private Methods"

        async Task<TModel> BuildEntityFromResultSets(DbDataReader reader)
        {

            TModel model = null;
            if ((reader != null) && (reader.HasRows))
            {
                model = ActivateInstanceOf<TModel>.Instance();
               
                // Entity
                await reader.ReadAsync();
                model.PopulateModel(reader);

                // Data
                if (await reader.NextResultAsync())
                {
                    var data = new List<EntityData>();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var entityData = new EntityData(reader);
                            data.Add(entityData);
                        }
                    }
                  
                    model.Data = data;

                }

            }

            return model;
        }

        async Task<int> InsertUpdateInternal(
            int id,
            int parentId,
            int featureId,
            int categoryId,
            string title,
            string alias,
            string message,
            string html,
            string messageAbstract,
            string urls,
            bool isHidden,
            bool isPrivate,
            bool isSpam,
            bool isPinned,
            bool isDeleted,
            bool isLocked,
            bool isClosed,
            int totalViews,
            int totalReplies,
            int totalAnswers,
            int totalParticipants,
            int totalReactions,
            int totalFollows,
            int totalReports,
            int totalStars,
            int totalRatings,
            int summedRating,
            int meanRating,
            int totalLinks,
            int totalImages,
            double dailyViews,
            double dailyReplies,
            double dailyAnswers,
            double dailyReactions,
            double dailyFollows,
            double dailyReports,
            double dailyStars,
            double dailyRatings,
            int sortOrder,
            string ipV4Address,
            string ipV6Address,
            int createdUserId,
            DateTimeOffset? createdDate,
            int editedUserId,
            DateTimeOffset? editedDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate,
            int lastReplyId,
            int lastReplyUserId,
            DateTimeOffset? lastReplyDate,
            IEnumerable<IEntityData> data)
        {

            var entityId = 0;
            using (var context = _dbContext)
            {
                entityId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntity",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ParentId", DbType.Int32, parentId),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("CategoryId", DbType.Int32, categoryId),
                        new DbParam("Title",DbType.String, 255, title.ToEmptyIfNull()),
                        new DbParam("Alias",DbType.String, 255, alias.ToEmptyIfNull()),
                        new DbParam("Message",DbType.String, message.ToEmptyIfNull()),
                        new DbParam("Html",DbType.String, html.ToEmptyIfNull()),
                        new DbParam("Abstract",DbType.String, 500, messageAbstract.ToEmptyIfNull()),
                        new DbParam("Urls", DbType.String, urls),
                        new DbParam("IsHidden", DbType.Boolean, isHidden),
                        new DbParam("IsPrivate", DbType.Boolean, isPrivate),
                        new DbParam("IsSpam", DbType.Boolean, isSpam),
                        new DbParam("IsPinned", DbType.Boolean, isPinned),
                        new DbParam("IsDeleted", DbType.Boolean, isDeleted),
                        new DbParam("IsLocked", DbType.Boolean, isLocked),
                        new DbParam("IsClosed", DbType.Boolean, isClosed),
                        new DbParam("TotalViews", DbType.Int32, totalViews),
                        new DbParam("TotalReplies", DbType.Int32, totalReplies),
                        new DbParam("TotalAnswers", DbType.Int32, totalAnswers),
                        new DbParam("TotalParticipants", DbType.Int32, totalParticipants),
                        new DbParam("TotalReactions", DbType.Int32, totalReactions),
                        new DbParam("TotalFollows", DbType.Int32, totalFollows),
                        new DbParam("TotalReports", DbType.Int32, totalReports),
                        new DbParam("TotalStars", DbType.Int32, totalStars),
                        new DbParam("TotalRatings", DbType.Int32, totalRatings),
                        new DbParam("SummedRating", DbType.Int32, summedRating),
                        new DbParam("MeanRating", DbType.Int32, meanRating),
                        new DbParam("TotalLinks", DbType.Int32, totalLinks),
                        new DbParam("TotalImages", DbType.Int32, totalImages),
                        new DbParam("DailyViews", DbType.Double, dailyViews),
                        new DbParam("DailyReplies", DbType.Double, dailyReplies),
                        new DbParam("DailyAnswers", DbType.Double, dailyAnswers),
                        new DbParam("DailyReactions", DbType.Double, dailyReactions),
                        new DbParam("DailyFollows", DbType.Double, dailyFollows),
                        new DbParam("DailyReports", DbType.Double, dailyReports),
                        new DbParam("DailyStars", DbType.Double, dailyStars),
                        new DbParam("DailyRatings", DbType.Double, dailyRatings),
                        new DbParam("SortOrder", DbType.Int32, sortOrder),
                        new DbParam("IpV4Address", DbType.String, 20, ipV4Address.ToEmptyIfNull()),
                        new DbParam("IpV6Address", DbType.String, 50, ipV6Address.ToEmptyIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("EditedUserId", DbType.Int32, editedUserId),
                        new DbParam("EditedDate", DbType.DateTimeOffset, editedDate),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("LastReplyId", DbType.Int32, lastReplyId),
                        new DbParam("LastReplyUserId", DbType.Int32, lastReplyUserId),
                        new DbParam("LastReplyDate", DbType.DateTimeOffset, lastReplyDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            //using (var context = _dbContext)
            //{
            //    entityId = await context.ExecuteScalarAsync<int>(
            //        CommandType.StoredProcedure,
            //        "InsertUpdateEntity",
            //        id,
            //        parentId,
            //        featureId,
            //        categoryId,
            //        title.ToEmptyIfNull().TrimToSize(255),
            //        alias.ToEmptyIfNull().TrimToSize(255),
            //        message.ToEmptyIfNull(),
            //        html.ToEmptyIfNull(),
            //        messageAbstract.ToEmptyIfNull().TrimToSize(500),
            //        urls.ToEmptyIfNull(),
            //        isHidden,
            //        isPrivate,
            //        isSpam,
            //        isPinned,
            //        isDeleted,
            //        isLocked,
            //        isClosed,
            //        totalViews,
            //        totalReplies,
            //        totalAnswers,
            //        totalParticipants,
            //        totalReactions,
            //        totalFollows,
            //        totalReports,
            //        totalStars,
            //        totalRatings,
            //        summedRating,
            //        meanRating,
            //        totalLinks,
            //        totalImages,
            //        dailyViews,
            //        dailyReplies,
            //        dailyAnswers,
            //        dailyReactions,
            //        dailyFollows,
            //        dailyReports,
            //        dailyStars,
            //        dailyRatings,
            //        sortOrder,
            //        createdUserId,
            //        createdDate.ToDateIfNull(),
            //        editedUserId,
            //        editedDate,
            //        modifiedUserId,
            //        modifiedDate,
            //        lastReplyId,
            //        lastReplyUserId,
            //        lastReplyDate.ToDateIfNull(),
            //        new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            //}
            
            // Add entity data
            if (entityId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.EntityId = entityId;
                        item.ModifiedDate = DateTimeOffset.UtcNow;
                        await _entityDataRepository.InsertUpdateAsync(item);
                    }
                }

            }

            return entityId;

        }
        
        #endregion

        public async Task<IEnumerable<TModel>> SelectByFeatureIdAsync(int featureId)
        {
            IList<TModel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectEntitiesByFeatureId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var entity = ActivateInstanceOf<TModel>.Instance();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;

                    }, new[]
                    {
                        new DbParam("FeatureId", DbType.Int32, featureId)
                    });

            }

            return output;

        }
    }

}