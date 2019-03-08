using System;
using System.Threading.Tasks;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.Services
{

    public interface ITagService
    {
        Task<IPagedResults<Tag>> GetTagsAsunc(TagIndexOptions options, PagerOptions pager);
    }

    public class TagService : ITagService
    {
        private readonly ITagStore<Tag> _tagStore;
        private readonly IFeatureFacade _featureFacade;

        public TagService(
            ITagStore<Tag> tagStore,
            IFeatureFacade featureFacade)
        {
            _tagStore = tagStore;
            _featureFacade = featureFacade;
        }

        public async Task<IPagedResults<Tag>> GetTagsAsunc(TagIndexOptions options, PagerOptions pager)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature == null)
            {
                return null;
            }

            return await _tagStore.QueryAsync()
                .Take(pager.Page, pager.Size)
                .Select<TagQueryParams>(q =>
                {
                   q.FeatureId.Equals(feature.Id);
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
