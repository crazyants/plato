using System.Collections.Generic;
using Plato.Discuss.Models;
using Plato.Internal.Models;

namespace Plato.Discuss.ViewModels
{
    public class TopicListItemViewModel
    {
        
        public int SelectedOffset { get; set; }

        public Topic Topic { get; set; }
        
        public ILabelBase Channel { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }
       
    }
}
