using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Labels.ViewModels
{
    
    public class LabelDropDownViewModel : LabelInputViewModel
    {

        public LabelIndexOptions Options { get; set; }

        public string LabelsJson { get; set; }

    }

    public class LabelInputViewModel
    {

        public string HtmlName { get; set; }

        [Required]
        [Display(Name = "label")]
        public IEnumerable<int> SelectedLabels { get; set; }

    }

}
