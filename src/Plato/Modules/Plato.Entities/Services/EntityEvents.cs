using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;

namespace Plato.Entities.Services
{
    public class EntityEvents
    {

        public event EntityEventHandler Creating;
        public event EntityEventHandler Created;
        public event EntityEventHandler Updating;
        public event EntityEventHandler Updated;
        public event EntityEventHandler Deleting;
        public event EntityEventHandler Deleted;

        public delegate void EntityEventHandler(object sender, EntityStoreEventArgs e);
        

    }
}
