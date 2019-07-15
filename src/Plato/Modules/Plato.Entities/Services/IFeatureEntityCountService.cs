using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Services
{
    public interface IFeatureEntityCountService
    {
        Task<IEnumerable<FeatureEntityCount>> GetResultsAsync(EntityIndexOptions options = null);

        IFeatureEntityCountService ConfigureDb(Action<IQueryOptions> configure);

        IFeatureEntityCountService ConfigureQuery(Action<FeatureEntityCountQueryParams> configure);
    }
}
