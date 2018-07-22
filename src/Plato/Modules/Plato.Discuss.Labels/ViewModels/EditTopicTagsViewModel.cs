﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Labels.ViewModels
{
    public class EditTopicTagsViewModel
    {
     
        [Required]
        [Display(Name = "channel")]
        public IEnumerable<int> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }
}