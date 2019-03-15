using Plato.Tags.Models;

namespace Plato.Tags.ViewModels
{
    public class TagListItemViewModel<TModel> where TModel : class, ITag
    {

        public TModel Tag { get; set; }

        public bool EnableEdit { get; set; }

    }
}
