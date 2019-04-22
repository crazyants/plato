using Plato.Entities.Models;
using Plato.Internal.Models.Users;

namespace Plato.Entities.ViewModels
{
    public class UserDisplayViewModel<TModel>  : UserDisplayViewModel where TModel : class
    {
        
        public EntityIndexViewModel<TModel> IndexViewModel { get; set; }

    }

    public class UserDisplayViewModel
    {
        public User User { get; set; }

        public FeatureEntityMetrics Metrics { get; set; } = new FeatureEntityMetrics();

    }
}
