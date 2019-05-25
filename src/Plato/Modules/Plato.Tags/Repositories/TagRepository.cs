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

        private readonly ILogger<TagRepository<TModel>> _logger;
        private readonly IDbContext _dbContext;
 
        public TagRepository(
            ILogger<TagRepository<TModel>> logger,
            IDbContext dbContext)
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
                tag = await context.ExecuteReaderAsync2<TModel>(
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
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

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
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteTagById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<TModel>> SelectByFeatureIdAsync(int featureId)
        {

            IList<TModel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IList<TModel>>(
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

                    }, new[]
                    {
                        new DbParam("FeatureId", DbType.Int32, featureId)
                    });

            }

            return output;
        }

        public async Task<TModel> SelectByNameAsync(string name)
        {
            TModel tag = null;
            using (var context = _dbContext)
            {
                tag = await context.ExecuteReaderAsync2<TModel>(
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
                    }, new[]
                    {
                        new DbParam("Name", DbType.String, 255, name)
                    });


            }

            return tag;
        }

        public async Task<TModel> SelectByNameNormalizedAsync(string nameNormalized)
        {
            TModel tag = null;
            using (var context = _dbContext)
            {
                tag = await context.ExecuteReaderAsync2<TModel>(
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
                    }, new []
                    {
                        new DbParam("NameNormalized", DbType.String, 255, nameNormalized), 
                    });
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
                tagId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateTag",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("NameNormalized", DbType.String, 255, nameNormalized),
                        new DbParam("Description", DbType.String, 500, description.ToEmptyIfNull()),
                        new DbParam("Alias", DbType.String, 255, alias),
                        new DbParam("TotalEntities", DbType.Int32, totalEntities),
                        new DbParam("TotalFollows", DbType.Int32, totalFollows),
                        new DbParam("LastSeenDate", DbType.DateTimeOffset, lastSeenDate.ToDateIfNull()),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return tagId;

        }
        
        #endregion

    }

}
