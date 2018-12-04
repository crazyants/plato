using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Discuss.Tags.ViewModels
{
    public class EditTopicTagsViewModel
    {

        [Required]
        [Display(Name = "tags")]
        public string Tags { get; set; }

        [Required]
        [Display(Name = "tag")]
        public IEnumerable<int> SelectedTags { get; set; }

        public string HtmlName { get; set; }

    }

}
