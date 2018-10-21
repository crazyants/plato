using System.Collections.Generic;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class HomeTopicViewModel
    {

        public HomeTopicViewModel(
            IPagedResults<Reply> replies,
            PagerOptions pager)
        {
            this.Replies = replies;
            this.PagerOpts = pager;
            this.PagerOpts.SetTotal(replies?.Total ?? 0);
        }

        public PagerOptions PagerOpts { get; set; }

        public IPagedResults<Reply> Replies { get; set; }
     
        public Topic Entity { get; set; }

        public IPagedResults<EntityUser> Users { get; set; }
        
    }

}
