using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Articles.Categories.ViewModels
{
    public class EditTopicChannelsViewModel
    {
     
        [Required]
        [Display(Name = "channel")]
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}
