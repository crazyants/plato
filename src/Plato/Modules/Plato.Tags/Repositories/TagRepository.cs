using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;


namespace Plato.Tags.Repositories
{
    public class TagRepository<TModel> : ITagRepository<TModel> where TModel : class, ITag
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<TagRepository<TModel>> _logger;

        public TagRepository(
            IDbContext dbContext,
            ILogger<TagRepository<TModel>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TModel> InsertUpdateAsync(TModel tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var id = await InsertUpdateInternal(
                tag.Id,
                tag.FeatureId,
                tag.Name,
                tag.NameNormalized,
                tag.Description,
                tag.Alias,
                tag.TotalEntities,
                tag.TotalFollows,
                tag.LastSeenDate,
                tag.CreatedUserId,
                tag.CreatedDate,
                tag.ModifiedUserId,
                tag.ModifiedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

   
        public async Task<TModel> SelectByIdAsync(int id)
        {
            TModel tag = null;
            using (var context = _dbContext)
            {
                tag = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectTagById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            tag = ActivateInstanceOf<TModel>.Instance();
                            tag.PopulateModel(reader);
                        }

                        return tag;
                    },
                    id);
              

            }

            return tag;

        }

        public async Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<TModel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectTagsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var tag = ActivateInstanceOf<TModel>.Instance();
                                tag.PopulateModel(reader);
                                output.Data.Add(tag);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;

                    },
                    inputParams);
            
            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting tag with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteTagById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<TModel>> SelectByFeatureIdAsync(int featureId)
        {

            IList<TModel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<TModel>>(
                    CommandType.StoredProcedure,
                    "SelectTagsByFeatureId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<TModel>();
                            while (await reader.ReadAsync())
                            {
                                var tag = ActivateInstanceOf<TModel>.Instance();
                                tag.PopulateModel(reader);
                                output.Add(tag);
                            }

                        }

                        return output;

                    },
                    featureId);
                
            }

            return output;
        }

        public async Task<TModel> SelectByNameAsync(string name)
        {
            TModel tag = null;
            using (var context = _dbContext)
            {
                tag = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectTagByName",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            tag = ActivateInstanceOf<TModel>.Instance();
                            tag.PopulateModel(reader);
                        }

                        return tag;
                    },
                    name
                );

               
            }

            return tag;
        }

        public async Task<TModel> SelectByNameNormalizedAsync(string nameNormalized)
        {
            TModel tag = null;
            using (var context = _dbContext)
            {
                tag = await context.ExecuteReaderAsync<TModel>(
                    CommandType.StoredProcedure,
                    "SelectTagByNameNormalized",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            tag = ActivateInstanceOf<TModel>.Instance();
                            tag.PopulateModel(reader);
                        }

                        return tag;
                    },
                    nameNormalized);
            }

            return tag;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int featureId,
            string name,
            string nameNormalized,
            string description,
            string alias,
            int totalEntities,
            int totalFollows,
            DateTimeOffset? lastSeenDate,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var tagId = 0;
            using (var context = _dbContext)
            {
                tagId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateTag",
                    id,
                    featureId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    nameNormalized.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(500),
                    alias.ToEmptyIfNull().TrimToSize(255),
                    totalEntities,
                    totalFollows,
                    lastSeenDate.ToDateIfNull(),
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return tagId;

        }
        
        #endregion

    }

}
