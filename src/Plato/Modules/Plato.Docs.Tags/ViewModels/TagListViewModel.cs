using System.Collections.Generic;
using Plato.Docs.Models;
using Plato.Tags.Models;

namespace Plato.Docs.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Doc Topic { get; set; }

        public DocComment Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
