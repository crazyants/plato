using System;
using Plato.Entities.Models;

namespace Plato.Entities.Models
{
    public class EntityEventArgs<TModel> where TModel : class
    {

        public bool Success { get; }

        public TModel Entity { get; }
        
        public EntityEventArgs(TModel entity)
        {
            Entity = entity;
        }

        public EntityEventArgs(TModel entity, bool success)
        {
            Entity = entity;
            Success = success;
        }

    }


    public class EntityReplyEventArgs<TModel> where TModel : class
    {
  
        public bool Success { get; }

        public Entity Entity { get; }

        public TModel EntityReply { get; }

        public EntityReplyEventArgs(bool success)
        {
            Success = success;
        }
        
        public EntityReplyEventArgs(Entity entity, TModel entityReply)
        {
            Entity = entity;
            EntityReply = entityReply;
        }

    }

}
