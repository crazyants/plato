using System.Collections.Generic;

namespace Plato.Discuss.Channels.ViewModels
{
    public class EditTopicChannelsViewModel
    {
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}
