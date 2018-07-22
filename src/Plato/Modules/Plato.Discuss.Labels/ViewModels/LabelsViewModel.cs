using System.Collections.Generic;
using Plato.Labels.Models;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelsViewModel
    {
        public IEnumerable<LabelBase> Labels { get; set; }
    }
}
