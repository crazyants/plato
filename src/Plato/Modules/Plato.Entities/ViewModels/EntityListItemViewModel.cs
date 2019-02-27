using System.Collections.Generic;
using Plato.Internal.Models;

namespace Plato.Entities.ViewModels
{
    public class EntityListItemViewModel<TModel> where TModel : class
    {

        public TModel Entity { get; set; }

        public ILabelBase Channel { get; set; }

        public IEnumerable<ILabelBase> Labels { get; set; }

        public EntityIndexOptions Options { get; set; }
    }

}
