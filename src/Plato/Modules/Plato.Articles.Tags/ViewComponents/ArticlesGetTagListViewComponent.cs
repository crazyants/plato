using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Navigation.Abstractions;
using Plato.Tags.ViewModels;
using Plato.Articles.Tags.Models;
using Plato.Tags.Services;

namespace Plato.Articles.Tags.ViewComponents
{
    public class ArticlesGetTagListViewComponent : ViewComponent
    {
        
        private readonly ITagService<Tag> _tagService;

        public ArticlesGetTagListViewComponent(
            ITagService<Tag> tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync(TagIndexOptions options, PagerOptions pager)
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

        async Task<TagIndexViewModel<Tag>> GetViewModel(TagIndexOptions options, PagerOptions pager)
        {

            // Get tags
            var results = await _tagService.GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TagIndexViewModel<Tag>()
            {
                Results = results,
                Options = options,
                Pager = pager
            };

        }

    }

}
