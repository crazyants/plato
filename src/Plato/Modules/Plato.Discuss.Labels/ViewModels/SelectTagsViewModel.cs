using System.Collections.Generic;

namespace Plato.Discuss.Labels.ViewModels
{
    public class SelectTagsViewModel
    {
        
        public IList<Selection<Models.Label>> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }


}
