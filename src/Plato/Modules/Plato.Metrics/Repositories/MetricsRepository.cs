using System;
using System.Collections.Generic;
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
                metric.AreaName,
                metric.ControllerName,
                metric.ActionName,
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteMetricById", id);
            }

            return success > 0 ? true : false;
        }
        
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string areaName,
            string controllerName,
            string actionName,
            string ipV4Address,
            string ipV6Address,
            string userAgent,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateMetric",
                    id,
                    areaName.TrimToSize(100).ToEmptyIfNull(),
                    controllerName.TrimToSize(100).ToEmptyIfNull(),
                    actionName.TrimToSize(100).ToEmptyIfNull(),
                    ipV4Address.TrimToSize(20).ToEmptyIfNull(),
                    ipV6Address.TrimToSize(50).ToEmptyIfNull(),
                    userAgent.TrimToSize(255).ToEmptyIfNull(),
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output)
                );
            }

            return emailId;

        }

        #endregion

    }

}
