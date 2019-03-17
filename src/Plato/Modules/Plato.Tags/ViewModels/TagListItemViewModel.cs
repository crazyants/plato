using Plato.Internal.Models.Tags;

namespace Plato.Tags.ViewModels
{
    public class TagListItemViewModel<TModel> where TModel : class, ITag
    {

        public TModel Tag { get; set; }

        public bool EnableEdit { get; set; }

    }
}
