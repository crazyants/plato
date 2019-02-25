using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.Services;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Tags.ViewComponents
{
    public class GetTagListViewComponent : ViewComponent
    {
        
        private readonly ITagService _tagService;

        public GetTagListViewComponent(
            ITagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TagIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TagIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            return View(await GetViewModel(options, pager));

        }

        async Task<TagIndexViewModel> GetViewModel(
            TagIndexOptions options,
            PagerOptions pager)
        {

            // Get tags
            var results = await _tagService.GetTagsAsunc(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TagIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}
