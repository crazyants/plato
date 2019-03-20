using Plato.Articles.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Tags.ViewModels
{
    public class TagDisplayViewModel
    {

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

    }

}
