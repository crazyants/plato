using System.Runtime.Serialization;
using Plato.Articles.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.ViewModels
{
    public class ArticleViewModel
    {
        
        public PagerOptions Pager { get; set; }
        
        public Article Article { get; set; }

        public IPagedResults<ArticleComment> Replies { get; set; }
     
        public TopicOptions Options { get; set; }
        
    }

    [DataContract]
    public class TopicOptions
    {
        
        public string Sort { get; set; } = "CreatedDate";

        public OrderBy Order { get; set; } = OrderBy.Asc;

        public InfiniteScrollOptions InfiniteScroll { get; set; }

        public TopicParams Params { get; set; }
        
        public TopicOptions()
        {
            Params = new TopicParams();
            InfiniteScroll = new InfiniteScrollOptions();
        }
    }

    [DataContract]
    public class TopicParams
    {

        public int EntityId { get; set; }

    }
}
