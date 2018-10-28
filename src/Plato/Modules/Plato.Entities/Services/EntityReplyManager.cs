using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Entities.Services
{
    public class EntityReplyManager<TReply> : IEntityReplyManager<TReply> where TReply : class, IEntityReply
    {

        public event EntityReplyEvents<TReply>.Handler Creating;
        public event EntityReplyEvents<TReply>.Handler Created;
        public event EntityReplyEvents<TReply>.Handler Updating;
        public event EntityReplyEvents<TReply>.Handler Updated;
        public event EntityReplyEvents<TReply>.Handler Deleting;
        public event EntityReplyEvents<TReply>.Handler Deleted;
     
        private readonly IBroker _broker;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IEntityReplyStore<TReply> _entityReplyStore;
        private readonly IContextFacade _contextFacade;

        public EntityReplyManager(
            IEntityReplyStore<TReply> entityReplyStore, 
            IBroker broker, IContextFacade contextFacade,
            IEntityStore<Entity> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _broker = broker;
        }

        public async Task<ICommandResult<TReply>> CreateAsync(TReply reply)
        {
            
            var result = new CommandResult<TReply>();


            if (reply.Id > 0)
            {
                return result.Failed(new CommandError($"{nameof(reply.Id)} cannot be greater than zero when creating a reply"));
            }

            if (reply.EntityId <= 0)
            {
                return result.Failed(new CommandError($"{nameof(reply.EntityId)} must must be greater than zero"));
            }
        
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                return result.Failed(new CommandError($"{nameof(reply.Message)} is required"));
            }

            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new CommandError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                reply.CreatedUserId = user.Id;
            }
     
            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
            // Raise creating event
            Creating?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, reply));

            // Invoke EntityReplyCreating subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, new MessageOptions()
            {
                Key = "EntityReplyCreating"
            }, reply))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }
            
            var newReply = await _entityReplyStore.CreateAsync(reply);
            if (newReply != null)
            {
                // Raise created event
                Created?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, newReply));

                // Invoke EntityReplyCreated subscriptions
                foreach (var handler in _broker.Pub(this, new MessageOptions()
                {
                    Key = "EntityReplyCreated"
                }, newReply))
                {
                    newReply = await handler.Invoke(new Message<TReply>(newReply, this));
                }
                
                return result.Success(newReply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the reply"));

        }

        public async Task<ICommandResult<TReply>> UpdateAsync(TReply reply)
        {

            var result = new CommandResult<TReply>();

            if (reply.Id <= 0)
            {
                return result.Failed(new CommandError($"{nameof(reply.Id)} must be a valid existing reply id"));
            }
            
            if (String.IsNullOrWhiteSpace(reply.Message))
            {
                return result.Failed(new CommandError($"{nameof(reply.Message)} is required"));
            }
            
            var entity = await _entityStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return result.Failed(new CommandError($"An entity with the Id '{reply.EntityId}' could not be found"));
            }
            
            // Update modified details
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                reply.ModifiedUserId = user.Id;
            }
            
            reply.ModifiedDate = DateTime.UtcNow;

            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
            // Raise updating event
            Updating?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, reply));

            // Invoke EntityReplyUpdating subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, new MessageOptions()
            {
                Key = "EntityReplyUpdating"
            }, reply))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }

            var updatedReply = await _entityReplyStore.UpdateAsync(reply);
            if (updatedReply != null)
            {
                // Raise Updated event
                Updated?.Invoke(this, new EntityReplyEventArgs<TReply>(entity, updatedReply));

                // Invoke EntityReplyUpdated subscriptions
                foreach (var handler in _broker.Pub<TReply>(this, new MessageOptions()
                {
                    Key = "EntityReplyUpdated"
                }, reply))
                {
                    updatedReply = await handler.Invoke(new Message<TReply>(updatedReply, this));
                }

                return result.Success(updatedReply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the reply."));

        }

        public async Task<ICommandResult<TReply>> DeleteAsync(TReply reply)
        {

            // Validate
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }
            
            var result = new CommandResult<TReply>();
            
            // Raise Deleting event
            Deleting?.Invoke(this, new EntityReplyEventArgs<TReply>(null, reply));

            // Invoke EntityReplyDeleting subscriptions
            foreach (var handler in _broker.Pub<TReply>(this, new MessageOptions()
            {
                Key = "EntityReplyDeleting"
            }, reply))
            {
                reply = await handler.Invoke(new Message<TReply>(reply, this));
            }
        
            var success = await _entityReplyStore.DeleteAsync(reply);
            if (success)
            {

                // Raise Deleted event
                Deleted?.Invoke(this, new EntityReplyEventArgs<TReply>(null, reply));

                // Invoke EntityReplyDeleted subscriptions
                foreach (var handler in _broker.Pub<TReply>(this, new MessageOptions()
                {
                    Key = "EntityReplyDeleted"
                }, reply))
                {
                    reply = await handler.Invoke(new Message<TReply>(reply, this));
                }
       
                return result.Success(reply);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the reply."));

        }
        
        #region "Private Methods"

        private async Task<string> ParseMarkdown(string message)
        {

            var output = string.Empty;
            foreach (var handler in _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, message))
            {
                output = await handler.Invoke(new Message<string>(message, this));
            }

            return output;

        }

        private async Task<string> ParseAbstract(string message)
        {

            var output = message.PlainTextulize().TrimToAround(225);
            foreach (var handler in _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseAbstract"
            }, message))
            {
                output = await handler.Invoke(new Message<string>(message, this));
            }

            return output;

        }

        #endregion


    }
}
