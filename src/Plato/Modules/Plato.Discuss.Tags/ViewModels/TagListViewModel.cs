using System.Collections.Generic;
using Plato.Discuss.Models;
using Plato.Tags.Models;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Topic Topic { get; set; }

        public Reply Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
