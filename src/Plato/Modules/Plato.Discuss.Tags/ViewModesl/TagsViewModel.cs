using System.Collections.Generic;
using Plato.Categories.Models;
using Plato.Discuss.Tags.Models;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagsViewModel
    {
        public IEnumerable<Tag> Channels { get; set; }
    }
}
