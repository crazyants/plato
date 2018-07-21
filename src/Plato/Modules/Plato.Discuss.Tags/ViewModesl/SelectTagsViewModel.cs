using System.Collections.Generic;
using Plato.Discuss.Tags.Models;

namespace Plato.Discuss.Tags.ViewModels
{
    public class SelectTagsViewModel
    {
        
        public IList<Selection<Tag>> SelectedChannels { get; set; }

        public string HtmlName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }


}
