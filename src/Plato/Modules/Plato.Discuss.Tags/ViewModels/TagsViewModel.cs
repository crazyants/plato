using System.Collections.Generic;
using Plato.Tags.Models;

namespace Plato.Discuss.Tags.ViewModels
{
    public class TagsViewModel
    {

        public int SelectedTagId { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }

}
