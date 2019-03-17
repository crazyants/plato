using System.Collections.Generic;
using Plato.Internal.Models.Tags;

namespace Plato.Tags.ViewModels
{
    public class TagsViewModel<TModel> where TModel : class, ITag
    {

        public int SelectedTagId { get; set; }

        public IEnumerable<TModel> Tags { get; set; }

    }
}
