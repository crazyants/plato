using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Stores
{

    public class FeatureEntityMetrics
    {
        public AggregatedResult<string> Metrics { get; set; }

    }

    //public class FeatureEntityMetric : IDbModel
    //{
    //    public string ModuleId { get; set; } = string.Empty;

    //    public int Count { get; set; }

    //    public void PopulateModel(IDataReader dr)
    //    {
    //        if (dr.ColumnIsNotNull("ModuleId"))
    //        {
    //            ModuleId = Convert.ToString(dr["ModuleId"]);
    //        }

    //        if (dr.ColumnIsNotNull("Count"))
    //        {
    //            Count = Convert.ToInt32(dr["Count"]);
    //        }
    //    }
    //}

    //public interface IFeatureEntityMetricsStore
    //{

    //    Task<FeatureEntityMetrics> GetEntityCountGroupedByFeature();

    //    Task<FeatureEntityMetrics> GetEntityCountGroupedByFeature(int userId);

    //}

    //public class FeatureEntityMetricsStore : IFeatureEntityMetricsStore
    //{

    //    private readonly IDbHelper _dbHelper;

    //    public FeatureEntityMetricsStore(IDbHelper dbHelper)
    //    {
    //        _dbHelper = dbHelper;
    //    }

    //    public async Task<FeatureEntityMetrics> GetEntityCountGroupedByFeature()
    //    {
    //        return await _dbHelper.ExecuteReaderAsync<FeatureEntityMetrics>(
    //            @"SELECT f.ModuleId AS ModuleId, COUNT(e.Id) AS Count
    //              FROM {prefix}_Entities e
    //              INNER JOIN {prefix}_ShellFeatures f ON f.Id = e.FeatureId               
    //              GROUP BY f.ModuleId",
    //            async reader =>
    //            {

    //                var metrics = new List<FeatureEntityMetric>();
    //                while (await reader.ReadAsync())
    //                {
    //                    var metric = new FeatureEntityMetric();
    //                    metric.PopulateModel(reader);
    //                    metrics.Add(metric);
    //                }

    //                return new FeatureEntityMetrics()
    //                {
    //                    Metrics = metrics,
    //                    Total = metrics.Sum(m => m.Count)
    //                };

    //            });

    //    }

    //    public async Task<FeatureEntityMetrics> GetEntityCountGroupedByFeature(int userId)
    //    {

    //        return await _dbHelper.ExecuteReaderAsync<FeatureEntityMetrics>(
    //            @"SELECT f.ModuleId AS ModuleId, COUNT(e.Id) AS Count
    //              FROM {prefix}_Entities e
    //              INNER JOIN {prefix}_ShellFeatures f ON f.Id = e.FeatureId
    //              WHERE e.CreatedUserId = {userId}
    //              GROUP BY f.ModuleId",
    //            new Dictionary<string, string>()
    //            {
    //                ["{userId}"] = userId.ToString()
    //            },
    //            async reader =>
    //            {
    //                var metrics = new List<FeatureEntityMetric>();
    //                while (await reader.ReadAsync())
    //                {
    //                    var metric = new FeatureEntityMetric();
    //                    metric.PopulateModel(reader);
    //                    metrics.Add(metric);
    //                }

    //                return new FeatureEntityMetrics()
    //                {
    //                    Metrics = metrics,
    //                    Total = metrics.Sum(m => m.Count)
    //                };

    //            });

    //    }

    //}

}
