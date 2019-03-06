using System.Collections.Generic;
using Plato.Articles.Categories.Models;

namespace Plato.Articles.Categories.ViewModels
{
    public class SelectChannelsViewModel
    {
        
        public IList<Selection<Channel>> SelectedChannels { get; set; }

        public IEnumerable<int> SelectedChannelIds { get; set; }
        
        public string HtmlName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }


}
