using System.Collections.Generic;
using Plato.Questions.Models;
using Plato.Tags.Models;

namespace Plato.Questions.Tags.ViewModels
{
    public class TagListViewModel
    {
        public Question Topic { get; set; }

        public Answer Reply { get; set; }

        public IEnumerable<EntityTag> Tags { get; set; }
    }

}
