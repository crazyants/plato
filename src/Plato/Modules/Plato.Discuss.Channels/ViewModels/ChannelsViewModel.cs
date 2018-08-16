using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelsViewModel
    {
        public int SelectedChannelId { get; set; }

        public bool EnableEditOptions { get; set; }

        public IEnumerable<CategoryBase> Channels { get; set; }
    }
}
