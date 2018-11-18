using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Notifications.Models;
using Plato.Notifications.Stores;

namespace Plato.Notifications.Services
{
    
    public class UserNotificationsManager : IUserNotificationsManager<UserNotification>
    {

        private readonly IUserNotificationsStore<UserNotification> _userNotificationsStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public UserNotificationsManager(
            IUserNotificationsStore<UserNotification> userNotificationsStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _userNotificationsStore = userNotificationsStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        public async Task<ICommandResult<UserNotification>> CreateAsync(UserNotification model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Update
            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }

            model.CreatedDate = DateTime.UtcNow;

            // Invoke UserNotificationCreating subscriptions
            foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationCreating"))
            {
                model = await handler.Invoke(new Message<UserNotification>(model, this));
            }

            // Create result
            var result = new CommandResult<UserNotification>();

            // Persist to database
            var newUserNotification = await _userNotificationsStore.CreateAsync(model);
            if (newUserNotification != null)
            {

                // Invoke UserNotificationCreated subscriptions
                foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationCreated"))
                {
                    newUserNotification = await handler.Invoke(new Message<UserNotification>(newUserNotification, this));
                }

                // Return success
                return result.Success(newUserNotification);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a user notification!"));

        }

        public async Task<ICommandResult<UserNotification>> UpdateAsync(UserNotification model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            if (model.UserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.UserId));
            }

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Invoke UserNotificationUpdating subscriptions
            foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationUpdating"))
            {
                model = await handler.Invoke(new Message<UserNotification>(model, this));
            }

            // Create result
            var result = new CommandResult<UserNotification>();

            // Persist to database
            var updatedUserNotification = await _userNotificationsStore.UpdateAsync(model);
            if (updatedUserNotification != null)
            {

                // Invoke UserNotificationUpdated subscriptions
                foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationUpdated"))
                {
                    updatedUserNotification = await handler.Invoke(new Message<UserNotification>(updatedUserNotification, this));
                }

                // Return success
                return result.Success(updatedUserNotification);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update a user notification!"));

        }

        public async Task<ICommandResult<UserNotification>> DeleteAsync(UserNotification model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke UserNotificationDeleting subscriptions
            foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationDeleting"))
            {
                model = await handler.Invoke(new Message<UserNotification>(model, this));
            }

            var result = new CommandResult<UserNotification>();
            if (await _userNotificationsStore.DeleteAsync(model))
            {

                // Invoke UserNotificationDeleted subscriptions
                foreach (var handler in _broker.Pub<UserNotification>(this, "UserNotificationDeleted"))
                {
                    model = await handler.Invoke(new Message<UserNotification>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete  a user notification!"));
            
        }

    }

}
