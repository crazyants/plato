using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.ViewComponents
{
    public class GetTopicListViewComponent : ViewComponent
    {
        
        private readonly ITopicService _topicService;

        public GetTopicListViewComponent(
            ITopicService topicService)
        {
            _topicService = topicService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TopicIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }
        
        async Task<TopicIndexViewModel> GetViewModel(
            TopicIndexOptions options,
            PagerOptions pager)
        {

            // Get results
            var results = await _topicService.GetTopicsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);
            
            // Return view model
            return new TopicIndexViewModel
            {
                Results = results,
                Options = options,
                Pager = pager
            }; 

        }

    }
    
}

