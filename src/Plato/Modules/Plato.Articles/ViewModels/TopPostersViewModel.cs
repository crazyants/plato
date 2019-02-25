using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Articles.ViewModels
{
    public class TopPostersViewModel
    {

        public IPagedResults<EntityUser> Users { get; set; }
    }
}
