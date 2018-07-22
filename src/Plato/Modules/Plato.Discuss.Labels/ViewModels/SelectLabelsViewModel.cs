using System.Collections.Generic;

namespace Plato.Discuss.Labels.ViewModels
{
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
