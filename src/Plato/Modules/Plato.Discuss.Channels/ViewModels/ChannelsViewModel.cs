using System.Collections.Generic;
using Plato.Categories.Models;

namespace Plato.Discuss.Channels.ViewModels
{
    public class ChannelsViewModel
    {
        public IEnumerable<Category> Channels { get; set; }
    }
}
