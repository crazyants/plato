using System;
using System.Collections.Generic;
using System.Text;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Search.ViewModels
{
    public class SearchIndexViewModel
    {

        public string Keywords { get; set; }


        public PagerOptions PagerOpts { get; set; }

        public ViewOptions ViewOpts { get; set; }

        public IPagedResults<Entity> Results { get; }


        public SearchIndexViewModel()
        {

        }

        public SearchIndexViewModel(
            IPagedResults<Entity> results,
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
