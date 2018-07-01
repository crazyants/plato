using Plato.Entities.Models;

namespace Plato.Entities.Stores
{
    public class EntityStoreEventArgs
    {

        public bool Success { get; set; }

        public Entity Entity { get; set; }

    }
}
