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
                throw new ArgumentNullException(nameof(reply));

            var id = await InsertUpdateInternal(
                reply.Id,
                reply.EntityId,
                reply.Message,
                reply.Html,
                reply.Abstract,
                reply.IsPrivate,
                reply.IsSpam,
                reply.IsPinned,
                reply.IsDeleted,
                reply.IsClosed,
                reply.CreatedUserId,
                reply.CreatedDate,
                reply.ModifiedUserId,
                reply.ModifiedDate,
                null);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<TModel> SelectByIdAsync(int id)
        {
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityReplyById", id);

                return await BuildObjectFromResultSets(reader);
            }
        }
        

        public async Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams)
        {
            PagedResults<TModel> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityRepliesPaged",
                    inputParams
                );

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
                if (reader.HasRows)
                {
                    reply.PopulateModel(reader);
                }
                }
            return reply;
        }


        async Task<int> InsertUpdateInternal(
          int id,
          int entityId,
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
                    entityId,
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
            
            return entityReplyId;

        }


        #endregion

    }


    //public class EntityReplyRepository : IEntityReplyRepository<EntityReply>
    //{

    //    #region "Constructor"

    //    private readonly IDbContext _dbContext;
    //    private readonly ILogger<EntityReplyRepository> _logger;

    //    public EntityReplyRepository(
    //        IDbContext dbContext, 
    //        ILogger<EntityReplyRepository> logger)
    //    {
    //        _dbContext = dbContext;
    //        _logger = logger;
    //    }

    //    #endregion


    //    #region "Implementation"

    //    public async Task<EntityReply> InsertUpdateAsync(EntityReply reply)
    //    {
    //        if (reply == null)
    //            throw new ArgumentNullException(nameof(reply));

    //        var id = await InsertUpdateInternal(
    //            reply.Id,
    //            reply.EntityId,
    //            reply.Message,
    //            reply.Html,
    //            reply.Abstract,
    //            reply.IsPrivate,
    //            reply.IsSpam,
    //            reply.IsPinned,
    //            reply.IsDeleted,
    //            reply.IsClosed,
    //            reply.CreatedUserId,
    //            reply.CreatedDate,
    //            reply.ModifiedUserId,
    //            reply.ModifiedDate,
    //            null);

    //        if (id > 0)
    //        {
    //            // return
    //            return await SelectByIdAsync(id);
    //        }

    //        return null;
    //    }

    //    public async Task<EntityReply> SelectByIdAsync(int id)
    //    {
    //        using (var context = _dbContext)
    //        {
    //            var reader = await context.ExecuteReaderAsync(
    //                CommandType.StoredProcedure,
    //                "SelectEntityReplyById", id);

    //            return await BuildObjectFromResultSets(reader);
    //        }
    //    }

    //    public async  Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
    //    {
    //        PagedResults<TModel> output = null;
    //        using (var context = _dbContext)
    //        {

    //            var reader = await context.ExecuteReaderAsync(
    //                CommandType.StoredProcedure,
    //                "SelectEntityRepliesPaged",
    //                inputParams
    //            );

    //            if ((reader != null) && (reader.HasRows))
    //            {
    //                output = new PagedResults<TModel>();
    //                while (await reader.ReadAsync())
    //                {
    //                    var reply = new EntityReply();
    //                    reply.PopulateModel(reader);
    //                    output.Data.Add((TModel)Convert.ChangeType(reply, typeof(TModel)));
    //                }

    //                if (await reader.NextResultAsync())
    //                {
    //                    await reader.ReadAsync();
    //                    output.PopulateTotal(reader);
    //                }
    //            }
    //        }

    //        return output;
    //    }


    //    public async Task<IPagedResults<EntityReply>> SelectAsync(params object[] inputParams)
    //    {
    //        PagedResults<EntityReply> output = null;
    //        using (var context = _dbContext)
    //        {

    //            var reader = await context.ExecuteReaderAsync(
    //                CommandType.StoredProcedure,
    //                "SelectEntityRepliesPaged",
    //                inputParams
    //            );

    //            if ((reader != null) && (reader.HasRows))
    //            {
    //                output = new PagedResults<EntityReply>();
    //                while (await reader.ReadAsync())
    //                {
    //                    var reply = new EntityReply();
    //                    reply.PopulateModel(reader);
    //                    output.Data.Add(reply);
    //                }

    //                if (await reader.NextResultAsync())
    //                {
    //                    await reader.ReadAsync();
    //                    output.PopulateTotal(reader);
    //                }
    //            }
    //        }

    //        return output;
    //    }


    //    public async Task<bool> DeleteAsync(int id)
    //    {
    //        if (_logger.IsEnabled(LogLevel.Information))
    //        {
    //            _logger.LogInformation($"Deleting entity reply with id: {id}");
    //        }

    //        var success = 0;
    //        using (var context = _dbContext)
    //        {
    //            success = await context.ExecuteScalarAsync<int>(
    //                CommandType.StoredProcedure,
    //                "DeleteEntityReplyById", id);
    //        }

    //        return success > 0 ? true : false;
    //    }

    //    #endregion
        
    //    #region "Private Methods"

    //    async Task<EntityReply> BuildObjectFromResultSets(DbDataReader reader)
    //    {

    //        EntityReply reply = null;
    //        if ((reader != null) && (reader.HasRows))
    //        {

    //            reply = new EntityReply();
    //            await reader.ReadAsync();
    //            if (reader.HasRows)
    //            {
    //                reply.PopulateModel(reader);
    //            }

    //            // data

    //            //if (await reader.NextResultAsync())
    //            //{
    //            //    if (reader.HasRows)
    //            //    {
    //            //        while (await reader.ReadAsync())
    //            //        {
    //            //            var entityData = new EntityData(reader);
    //            //            entity.Data.Add(entityData);
    //            //        }

    //            //    }
    //            //}

    //            //if (await reader.NextResultAsync())
    //            //{
    //            //    if (reader.HasRows)
    //            //    {
    //            //        await reader.ReadAsync();
    //            //        user.Detail = new UserDetail(reader);
    //            //    }
    //            //}

    //            //// roles

    //            //if (await reader.NextResultAsync())
    //            //{
    //            //    if (reader.HasRows)
    //            //    {
    //            //        while (await reader.ReadAsync())
    //            //        {
    //            //            user.UserRoles.Add(new Role(reader));
    //            //        }
    //            //    }
    //            //}

    //        }
    //        return reply;
    //    }


    //    async Task<int> InsertUpdateInternal(
    //      int id,
    //      int entityId,
    //      string message,
    //      string html,
    //      string messageAbstract,
    //      bool isPrivate,
    //      bool isSpam,
    //      bool isPinned,
    //      bool isDeleted,
    //      bool isClosed,
    //      int createdUserId,
    //      DateTime? createdDate,
    //      int modifiedUserId,
    //      DateTime? modifiedDate,
    //      IEnumerable<EntityData> data)
    //    {

    //        var entityReplyId = 0;
    //        using (var context = _dbContext)
    //        {

    //            context.OnException += (sender, args) =>
    //            {
    //                if (_logger.IsEnabled(LogLevel.Error))
    //                    _logger.LogInformation(
    //                        id == 0
    //                            ? $"Insert for entity reply with entityId '{entityId}' failed with the following error '{args.Exception.Message}'"
    //                            : $"Update for entity reply with Id {id} failed with the following error {args.Exception.Message}");
    //                throw args.Exception;
    //            };

    //            entityReplyId = await context.ExecuteScalarAsync<int>(
    //                CommandType.StoredProcedure,
    //                "InsertUpdateEntityReply",
    //                id,
    //                entityId,
    //                message.ToEmptyIfNull(),
    //                html.ToEmptyIfNull(),
    //                messageAbstract.ToEmptyIfNull().TrimToSize(500),
    //                isPrivate,
    //                isSpam,
    //                isPinned,
    //                isDeleted,
    //                isClosed,
    //                createdUserId,
    //                createdDate.ToDateIfNull(),
    //                modifiedUserId,
    //                modifiedDate.ToDateIfNull());
    //        }

    //        // Add entity data
    //        //if (entityId > 0)
    //        //{
    //        //    if (data != null)
    //        //    {
    //        //        foreach (var item in data)
    //        //        {
    //        //            item.EntityId = entityId;
    //        //            await _entityDataRepository.InsertUpdateAsync(item);
    //        //        }
    //        //    }

    //        //}

    //        return entityReplyId;

    //    }


    //    #endregion

    //}

}
