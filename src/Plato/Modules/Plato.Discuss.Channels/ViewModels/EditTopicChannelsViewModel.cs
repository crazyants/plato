using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Channels.ViewModels
{
    public class EditTopicChannelsViewModel
    {

        [Required]
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}
