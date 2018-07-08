using System.ComponentModel.DataAnnotations;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class HomeTopicViewModel
    {

        public HomeTopicViewModel()
        {

        }

        public HomeTopicViewModel(
            IPagedResults<EntityReply> results,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

        public PagerOptions PagerOpts { get; set; }

        public IPagedResults<EntityReply> Results { get; set; }
        
        public FilterOptions FilterOpts { get; set; }
        
        public Entity Entity { get; set; }
        
    }


    //public class NewEntityReplyViewModel
    //{
   
    //    [Required]
    //    public string Message { get; set; }

    //}

}
