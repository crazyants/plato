using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Tags.ViewModels
{
    public class EditTopicTagsViewModel
    {
     
        [Required]
        [Display(Name = "channel")]
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}
