using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListViewComponent : ViewComponent
    {

  
        private readonly ITopicService _topicService;

        public TopicListViewComponent(
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


            var model = await GetIndexViewModel(options, pager);

            return View(model);
        }
        
        async Task<TopicIndexViewModel> GetIndexViewModel(
            TopicIndexOptions options,
            PagerOptions pager)
        {
            var topics = await _topicService.Get(options, pager);
            return new TopicIndexViewModel(
                topics,
                options,
                pager);
        }

    }


}

