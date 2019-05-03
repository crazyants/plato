using System.Collections.Generic;
using Plato.Ideas.Models;
using Plato.Tags.Models;

namespace Plato.Ideas.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Idea Topic { get; set; }

        public IdeaComment Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
