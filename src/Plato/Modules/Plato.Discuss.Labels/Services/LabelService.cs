using System;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.Services
{

    public class LabelService : ILabelService
    {

        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;

        public LabelService(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
        }

        public async Task<IPagedResults<Label>> GetLabelsAsunc(LabelIndexOptions options, PagerOptions pager)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");

            return await _labelStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<LabelQueryParams>(q =>
                {
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (!String.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }
    }

}
