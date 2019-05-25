using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Metrics.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Metrics.Repositories
{

    public class MetricsRepository : IMetricsRepository<Metric>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<MetricsRepository> _logger;

        public MetricsRepository(
            IDbContext dbContext,
            ILogger<MetricsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Metric> InsertUpdateAsync(Metric metric)
        {
            if (metric == null)
            {
                throw new ArgumentNullException(nameof(metric));
            }

            var id = await InsertUpdateInternal(
                metric.Id,
                metric.FeatureId,
                metric.Title,
                metric.Url,
                metric.IpV4Address,
                metric.IpV6Address,
                metric.UserAgent,
                metric.CreatedUserId,
                metric.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Metric> SelectByIdAsync(int id)
        {
            Metric output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<Metric>(
                    CommandType.StoredProcedure,
                    "SelectMetricById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            output = new Metric();
                            output.PopulateModel(reader);
                        }

                        return output;
                    },
                    id);

            }

            return output;

        }

        public async Task<IPagedResults<Metric>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<Metric> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<Metric>>(
                    CommandType.StoredProcedure,
                    "SelectMetricsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Metric>();
                            while (await reader.ReadAsync())
                            {
                                var metric = new Metric();
                                metric.PopulateModel(reader);
                                output.Data.Add(metric);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
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
                _logger.LogInformation($"Deleting metric with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteMetricById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int featureId,
            string title,
            string url,
            string ipV4Address,
            string ipV6Address,
            string userAgent,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateMetric",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("Title", DbType.String, 255, title),
                        new DbParam("Url", DbType.String, 255, url),
                        new DbParam("IpV4Address", DbType.String, 20, ipV4Address),
                        new DbParam("IpV6Address", DbType.String, 50, ipV6Address),
                        new DbParam("UserAgent", DbType.String, 255, userAgent),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return emailId;

        }

        #endregion

    }

}
