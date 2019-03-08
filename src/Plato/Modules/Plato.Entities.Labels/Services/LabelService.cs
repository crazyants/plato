using System;
using System.Threading.Tasks;
using Plato.Entities.Labels.Models;
using Plato.Entities.Labels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Entities.Labels.Stores;

namespace Plato.Entities.Labels.Services
{

    public class LabelService<TModel> : ILabelService<TModel> where TModel : class, ILabel
    {

        private readonly ILabelStore<TModel> _labelStore;
        
        public LabelService(
            ILabelStore<TModel> labelStore)
        {
            _labelStore = labelStore;
        }

        public async Task<IPagedResults<TModel>> GetResultsAsync(LabelIndexOptions options, PagerOptions pager)
        {

          
            return await _labelStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<LabelQueryParams>(q =>
                {
                    if (options.FeatureId > 0)
                    {
                        q.FeatureId.Equals(options.FeatureId);
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
