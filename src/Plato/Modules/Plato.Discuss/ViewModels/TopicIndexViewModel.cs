using Plato.Discuss.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class TopicIndexViewModel
    {

        public IPagedResults<Topic> Results { get; }

        public PagerOptions PagerOpts { get; set; }
        
        public ViewOptions ViewOpts { get; set; }
        
        public TopicIndexViewModel()
        {

        }

        public TopicIndexViewModel(
            IPagedResults<Topic> results,
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.ViewOpts = viewOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }


    }

    public class ViewOptions
    {
        public string Search { get; set; }

        public int ChannelId { get; set; }

        public EntityOrder Order { get; set; }

    }

    public enum EntityOrder
    {
        Username,
        Email
    }
    

}
