using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.ViewModels
{
    public class TopPostersViewModel
    {

        public IPagedResults<EntityUser> Users { get; set; }
    }
}
