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
                reply.IsPrivate,
                reply.IsSpam,
                reply.IsPinned,
                reply.IsDeleted,
                reply.IsClosed,
                reply.IsAnswer,
                reply.TotalReactions,
                reply.TotalReports,
                reply.TotalLinks,
                reply.TotalImages,
                reply.CreatedUserId,
                reply.CreatedDate,
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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyById", id);
                output = await BuildObjectFromResultSets(reader);
            }

            return output;

        }
        

        public async Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams)
        {
            PagedResults<TModel> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityRepliesPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<TModel>();
                    while (await reader.ReadAsync())
                    {
                        var reply = ActivateInstanceOf<TModel>.Instance();
                        reply.PopulateModel(reader);
                        output.Data.Add(reply);
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
          bool isPrivate,
          bool isSpam,
          bool isPinned,
          bool isDeleted,
          bool isClosed,
          bool isAnswer,
          int totalReactions,
          int totalReports,
          int totalLinks,
          int totalImages,
          int createdUserId,
          DateTimeOffset? createdDate,
          int modifiedUserId,
          DateTimeOffset? modifiedDate,
          IEnumerable<EntityData> data)
        {

            var entityReplyId = 0;
            using (var context = _dbContext)
            {

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for entity reply with entityId '{entityId}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for entity reply with Id {id} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

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
                    isPrivate,
                    isSpam,
                    isPinned,
                    isDeleted,
                    isClosed,
                    isAnswer,
                    totalReactions,
                    totalReports,
                    totalLinks,
                    totalImages,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }
            
            return entityReplyId;

        }


        #endregion

    }
    
}
