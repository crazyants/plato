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
    public class EntityReplyRepository<TModel> : IEntityReplyRepository<TModel> where TModel : class, IEntityReply
    {

        #region "Constructor"
        
        private readonly IEntityReplyDataRepository<IEntityReplyData> _entityReplyDataRepository;
        private readonly ILogger<EntityReplyRepository<TModel>> _logger;
        private readonly IDbContext _dbContext;

        public EntityReplyRepository(
            IEntityReplyDataRepository<IEntityReplyData> entityReplyDataRepository,
            ILogger<EntityReplyRepository<TModel>> logger,
            IDbContext dbContext)
        {
            _entityReplyDataRepository = entityReplyDataRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"

        public async Task<TModel> InsertUpdateAsync(TModel reply)
        {
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }

            var id = await InsertUpdateInternal(
                reply.Id,
                reply.ParentId,
                reply.EntityId,
                reply.Message,
                reply.Html,
                reply.Abstract,
                reply.Urls,
                reply.IsHidden,
                reply.IsSpam,
                reply.IsPinned,
                reply.IsDeleted,
                reply.IsClosed,
                reply.IsAnswer,
                reply.TotalReactions,
                reply.TotalReports,
                reply.TotalRatings,
                reply.SummedRating,
                reply.MeanRating,
                reply.TotalLinks,
                reply.TotalImages,
                reply.IpV4Address,
                reply.IpV6Address,
                reply.CreatedUserId,
                reply.CreatedDate,
                reply.EditedUserId,
                reply.EditedDate,
                reply.ModifiedUserId,
                reply.ModifiedDate,
                reply.Data);
            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<TModel> SelectByIdAsync(int id)
        {

            TModel output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyById",
                    async reader => await BuildFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return output;

        }
        

        public async Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            PagedResults<TModel> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityRepliesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var reply = ActivateInstanceOf<TModel>.Instance();
                                reply.PopulateModel(reader);
                                output.Data.Add(reply);
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
                _logger.LogInformation($"Deleting entity reply with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityReplyById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<TModel> BuildFromResultSets(DbDataReader reader)
        {

            TModel model = null;
            if ((reader != null) && (reader.HasRows))
            {

                model = ActivateInstanceOf<TModel>.Instance();

                await reader.ReadAsync();
                model.PopulateModel(reader);

                // Data
                if (await reader.NextResultAsync())
                {
                    var data = new List<EntityReplyData>();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var entityData = new EntityReplyData(reader);
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
          int entityId,
          string message,
          string html,
          string messageAbstract,
          string urls,
          bool isHidden,
          bool isSpam,
          bool isPinned,
          bool isDeleted,
          bool isClosed,
          bool isAnswer,
          int totalReactions,
          int totalReports,
          int totalRatings,
          int summedRating,
          int meanRating,
          int totalLinks,
          int totalImages,
          string ipV4Address,
          string ipV6Address,
          int createdUserId,
          DateTimeOffset? createdDate,
          int editedUserId,
          DateTimeOffset? editedDate,
          int modifiedUserId,
          DateTimeOffset? modifiedDate,
          IEnumerable<IEntityReplyData> data)
        {

            var entityReplyId = 0;
            using (var context = _dbContext)
            {
                entityReplyId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityReply",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id",DbType.Int32, id),
                        new DbParam("ParentId",DbType.Int32, parentId),
                        new DbParam("EntityId",DbType.Int32, entityId),
                        new DbParam("Message", DbType.String,  message.ToEmptyIfNull()),
                        new DbParam("Html", DbType.String, html.ToEmptyIfNull()),
                        new DbParam("Abstract", DbType.String, 500, messageAbstract.ToEmptyIfNull()),
                        new DbParam("Urls", DbType.String, urls.ToEmptyIfNull()),
                        new DbParam("IsHidden", DbType.Boolean, isHidden),
                        new DbParam("IsSpam", DbType.Boolean, isSpam),
                        new DbParam("IsPinned", DbType.Boolean, isPinned),
                        new DbParam("IsDeleted", DbType.Boolean, isDeleted),
                        new DbParam("IsClosed", DbType.Boolean, isClosed),
                        new DbParam("IsAnswer", DbType.Boolean, isAnswer),
                        new DbParam("TotalReactions", DbType.Int32, totalReactions),
                        new DbParam("TotalReports", DbType.Int32, totalReports),
                        new DbParam("TotalRatings", DbType.Int32, totalRatings),
                        new DbParam("SummedRating", DbType.Int32, summedRating),
                        new DbParam("MeanRating", DbType.Int32, meanRating),
                        new DbParam("TotalLinks", DbType.Int32, totalLinks),
                        new DbParam("TotalImages", DbType.Int32, totalImages),
                        new DbParam("IpV4Address", DbType.String, 20, ipV4Address.ToEmptyIfNull()),
                        new DbParam("IpV6Address", DbType.String, 50, ipV6Address.ToEmptyIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("EditedUserId", DbType.Int32, editedUserId),
                        new DbParam("EditedDate", DbType.DateTimeOffset, editedDate),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId",DbType.Int32, ParameterDirection.Output)
                    });
            }
            
            // Add entity data
            if (entityReplyId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.ReplyId = entityReplyId;
                        item.ModifiedDate = DateTimeOffset.UtcNow;
                        await _entityReplyDataRepository.InsertUpdateAsync(item);
                    }
                }

            }


            return entityReplyId;

        }
        
        #endregion

    }
    
}
