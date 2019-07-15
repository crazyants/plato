using Plato.Entities.Models;

namespace Plato.Entities.ViewModels
{
    public class CoreIndexViewModel
    {

        public EntityIndexViewModel<Entity> Latest { get; set; }

        public EntityIndexViewModel<Entity> Popular { get; set; }

        public FeatureEntityCounts Counts { get; set; }

    }

}
