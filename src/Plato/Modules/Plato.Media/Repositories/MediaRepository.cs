using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Media.Repositories
{

    public class MediaRepository : IMediaRepository<Models.Media>
    {
        #region "Constructor"

        private readonly IDbContext _dbContext;
        private readonly ILogger<MediaRepository> _logger;

        public MediaRepository(
            IDbContext dbContext,
            ILogger<MediaRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"

        public async Task<IPagedResults<Models.Media>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<Models.Media> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<Models.Media>>(
                    CommandType.StoredProcedure,
                    "SelectMediaPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Models.Media>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.Media();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
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
                _logger.LogInformation($"Deleting media with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteMediaById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<Models.Media> InsertUpdateAsync(Models.Media media)
        {
            var id = await InsertUpdateInternal(
                media.Id,
                media.Name,
                media.ContentBlob,
                media.ContentType,
                media.ContentLength,
                media.CreatedUserId,
                media.CreatedDate,
                media.ModifiedUserId,
                media.ModifiedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<Models.Media> SelectByIdAsync(int id)
        {
            Models.Media media = null;
            using (var context = _dbContext)
            {
                media = await context.ExecuteReaderAsync<Models.Media>(
                    CommandType.StoredProcedure,
                    "SelectMediaById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            media = new Models.Media(reader);
                        }

                        return media;
                    },
                    id);

            }

            return media;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            string name,
            byte[] contentBlob,
            string contentType,
            long contentLength,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var mediaId = 0;
            using (var context = _dbContext)
            {
                mediaId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateMedia",
                    id,
                    name.ToEmptyIfNull().ToSafeFileName().TrimToSize(255),
                    contentBlob ?? new byte[0], // don't allow nulls so we can determine parameter type
                    contentType.ToEmptyIfNull().TrimToSize(75),
                    contentLength,
                    createdUserId, 
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return mediaId;

        }

        #endregion
        
    }
}
