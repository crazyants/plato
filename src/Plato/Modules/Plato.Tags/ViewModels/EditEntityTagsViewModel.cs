using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Tags.ViewModels
{
    public class EditEntityTagsViewModel
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
