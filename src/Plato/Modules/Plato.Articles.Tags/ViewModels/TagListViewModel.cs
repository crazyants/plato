using System.Collections.Generic;
using Plato.Articles.Models;
using Plato.Tags.Models;

namespace Plato.Articles.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Article Topic { get; set; }

        public Comment Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
