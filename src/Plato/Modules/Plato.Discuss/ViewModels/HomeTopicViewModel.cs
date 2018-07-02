using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewModels
{
    public class HomeTopicViewModel
    {

        public PagerOptions PagerOpts { get; set; }

        public IPagedResults<Entity> Results { get; set; }


        public string EditorHtmlName { get; set; }


        public NewEntityReplyViewModel NewEntityReply { get; set; }

        public FilterOptions FilterOpts { get; set; }

        public HomeTopicViewModel()
        {

        }

        public HomeTopicViewModel(
            IPagedResults<Entity> results,
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            this.Results = results;
            this.FilterOpts = filterOptions;
            this.PagerOpts = pagerOptions;
            this.PagerOpts.SetTotal(results?.Total ?? 0);
        }

    }


    public class NewEntityReplyViewModel
    {

        [Required]
        public string Title { get; set; }

        public string Message { get; set; }


    }

}
