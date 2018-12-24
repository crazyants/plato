using System.Runtime.Serialization;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class TopicViewModel
    {
        
        public PagerOptions Pager { get; set; }

        public Topic Topic { get; set; }

        public IPagedResults<Reply> Replies { get; set; }
     
        public TopicOptions Options { get; set; }
        
    }

    [DataContract]
    public class TopicOptions
    {

        public TopicParams Params { get; set; }

        public TopicOptions()
        {
            Params = new TopicParams();
        }
    }

    [DataContract]
    public class TopicParams
    {

        public int TopicId { get; set; }

    }
}
