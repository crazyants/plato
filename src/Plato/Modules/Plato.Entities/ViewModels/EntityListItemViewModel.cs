using System.Collections.Generic;
using Plato.Internal.Models;
using Plato.Internal.Models.Tags;

namespace Plato.Entities.ViewModels
{
    public class EntityListItemViewModel<TModel> where TModel : class
    {

        public TModel Entity { get; set; }

        public ILabelBase Channel { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }

        public IEnumerable<ITag> Tags { get; set; }

        public EntityIndexOptions Options { get; set; }

    }

}
