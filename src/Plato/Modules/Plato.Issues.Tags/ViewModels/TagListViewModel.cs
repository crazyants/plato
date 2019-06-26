using System.Collections.Generic;
using Plato.Issues.Models;
using Plato.Tags.Models;

namespace Plato.Issues.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Issue Topic { get; set; }

        public Comment Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
