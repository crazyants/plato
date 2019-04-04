using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{
    public interface INestable<TModel> where TModel : class
    {

        IEnumerable<TModel> Children { get; set; }

        TModel Parent { get; set; }

        int Depth { get; set; }

        int SortOrder { get; set; }

    }

}
