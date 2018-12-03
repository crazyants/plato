using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Repositories
{
    public class TagRepository : ITagRepository<Tag>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(
            IDbContext dbContext,
            ILogger<TagRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Tag> InsertUpdateAsync(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            var id = await InsertUpdateInternal(
                tag.Id,
                tag.FeatureId,
                tag.Name,
                tag.Alias,
                tag.TotalEntities,
                tag.TotalFollows,
                tag.CreatedUserId,
                tag.CreatedDate);

            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;

        }

        public async Task<Tag> SelectByIdAsync(int id)
        {
            Tag tag = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectTagById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    tag = new Tag();
                    tag.PopulateModel(reader);
                }

            }

            return tag;

        }

        public async Task<IPagedResults<Tag>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Tag> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectTagsPaged",
                    inputParams);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<Tag>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new Tag();
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

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int featureId,
            string name,
            string alias,
            int totalEntities,
            int totalFollows,
            int createdUserId,
            DateTimeOffset? createdDate)
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
                    alias.ToEmptyIfNull().TrimToSize(255),
                    totalEntities,
                    totalFollows,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return tagId;

        }


        #endregion

    }

}
