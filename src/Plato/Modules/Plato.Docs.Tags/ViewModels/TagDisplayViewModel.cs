using Plato.Docs.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Tags.ViewModels
{
    public class TagDisplayViewModel
    {

        public PagerOptions Pager { get; set; }

        public EntityIndexOptions Options { get; set; }

    }

}
