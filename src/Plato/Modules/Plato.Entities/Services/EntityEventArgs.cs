using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public class EntityEventArgs
    {

        public bool Success { get; set; }

        public Entity Entity { get; set; }

    }


    public class EntityReplyEventArgs
    {

        public bool Success { get; set; }

        public Entity Entity { get; set; }

        public EntityReply EntityReply { get; set; }

    }

}
