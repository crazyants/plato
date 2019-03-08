using Plato.Labels.Models;

namespace Plato.Labels.ViewModels
{
    public class LabelListItemViewModel<TModel> where TModel : class, ILabel
    {
        public TModel Label { get; set; }

        public bool EnableEdit { get; set; }

    }
}
