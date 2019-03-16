using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.ViewModels
{
    

    public class EntityParticipantsViewModel
    {

        public IPagedResults<EntityUser> Users { get; set; }
    }
}
