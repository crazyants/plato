using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Labels.ViewModels
{
    
    public class SelectLabelsViewModel
    {

        public string LabelsJson { get; set; }
    
        public string HtmlName { get; set; }

        [Required]
        [Display(Name = "label")]
        public IEnumerable<int> SelectedLabels { get; set; }
    }

}
