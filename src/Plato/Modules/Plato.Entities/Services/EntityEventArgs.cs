using System;
using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public class EntityEventArgs
    {

        public bool Success { get; set; }

        public Entity Entity { get; set; }

    }


    public class EntityReplyEventArgs : EventArgs
    {
  
        public bool Success { get; }

        public Entity Entity { get; }

        public EntityReply EntityReply { get; }

        public EntityReplyEventArgs(bool success)
        {
            Success = success;
        }
        
        public EntityReplyEventArgs(Entity entity, EntityReply entityReply)
        {
            Entity = entity;
            EntityReply = entityReply;
        }

    }

}
