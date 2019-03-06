using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelListViewModel
    {
    
        public CategoryIndexOptions Options { get; set; } = new CategoryIndexOptions();

        public IEnumerable<CategoryBase> Channels { get; set; }

    }

}
