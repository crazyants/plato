using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;
using Plato.Mentions.Models;
using Plato.Mentions.Stores;

namespace Plato.Mentions.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {

        private readonly IBroker _broker;
        private readonly IEntityMentionsStore<EntityMention> _entityMentionsStore;


        public EntitySubscriber(
            IBroker broker,
            IEntityMentionsStore<EntityMention> entityMentionsStore)
        {
            _broker = broker;
            _entityMentionsStore = entityMentionsStore;
       
        }
        
        #region "Implementation"

        public void Subscribe()
        {
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {

        
        

            return entity;

        }

        #endregion

    }
}
