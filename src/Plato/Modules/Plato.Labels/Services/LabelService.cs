using System;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;

namespace Plato.Labels.Services
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
                .Take(pager.Page, pager.Size)
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
