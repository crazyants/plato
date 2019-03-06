using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Articles.Categories.ViewModels
{
    public class ChannelListViewModel
    {
        public int SelectedChannelId { get; set; }

        public ChannelIndexOptions ChannelIndexOpts { get; set; }

        public IEnumerable<CategoryBase> Channels { get; set; }

    }
    

}
