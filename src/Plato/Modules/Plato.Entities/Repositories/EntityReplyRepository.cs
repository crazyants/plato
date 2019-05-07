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

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityReplyRepository<TModel>> _logger;

        public EntityReplyRepository(
            IDbContext dbContext,
            ILogger<EntityReplyRepository<TModel>> logger)
        {
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
                reply.CreatedUserId,
                reply.CreatedDate,
                reply.EditedUserId,
                reply.EditedDate,
                reply.ModifiedUserId,
                reply.ModifiedDate,
                null);
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
                    async reader => await BuildObjectFromResultSets(reader), id);
            }

            return output;

        }
        

        public async Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams)
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
                    inputParams);
              
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
                    "DeleteEntityReplyById", id);
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<TModel> BuildObjectFromResultSets(DbDataReader reader)
        {

            TModel reply = null;
            if ((reader != null) && (reader.HasRows))
            {
                reply = ActivateInstanceOf<TModel>.Instance();
                await reader.ReadAsync();
                reply.PopulateModel(reader);
            }
            return reply;

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
          int createdUserId,
          DateTimeOffset? createdDate,
          int editedUserId,
          DateTimeOffset? editedDate,
          int modifiedUserId,
          DateTimeOffset? modifiedDate,
          IEnumerable<EntityData> data)
        {

            var entityReplyId = 0;
            using (var context = _dbContext)
            {
                entityReplyId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityReply",
                    id,
                    parentId,
                    entityId,
                    message.ToEmptyIfNull(),
                    html.ToEmptyIfNull(),
                    messageAbstract.ToEmptyIfNull().TrimToSize(500),
                    urls.ToEmptyIfNull(),
                    isHidden,
                    isSpam,
                    isPinned,
                    isDeleted,
                    isClosed,
                    isAnswer,
                    totalReactions,
                    totalReports,
                    totalRatings,
                    summedRating,
                    meanRating,
                    totalLinks,
                    totalImages,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    editedUserId,
                    editedDate,
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
            
            return entityReplyId;

        }
        
        #endregion

    }
    
}
