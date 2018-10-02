using System.Collections.Generic;
using Plato.Labels.Models;

namespace Plato.Discuss.Labels.ViewModels
{


    public class SelectLabelsLookUpViewModel
    {

        public string LabelsJson { get; set; }
    
        public string HtmlName { get; set; }

    }

    public class SelectLabelsViewModel
    {
        
        public IList<Selection<Models.Label>> SelectedLabels { get; set; }

        public string HtmlName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }


}
