using System.Collections.Generic;
using Plato.Labels.Models;

namespace Plato.Labels.ViewModels
{
    public class LabelsViewModel<TModel> where TModel : class, ILabel
    {

        public int SelectedLabelId { get; set; }

        public IEnumerable<TModel> Labels { get; set; }
    }
}
