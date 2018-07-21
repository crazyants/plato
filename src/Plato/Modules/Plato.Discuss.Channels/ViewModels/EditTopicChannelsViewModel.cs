using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Channels.ViewModels
{
    public class EditTopicChannelsViewModel
    {

        public int EntityId { get; set; }

        [Required]
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}
