using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Discuss.StopForumSpam.SpamOperators
{

    public class TopicOperator : ISpamOperatorProvider<Topic>
    {

        private readonly ISpamChecker _spamChecker;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityStore<Topic> _topicStore;

        public TopicOperator(
            ISpamChecker spamChecker,
            IPlatoUserStore<User> platoUserStore,
            IEntityStore<Topic> topicStore)
        {
            _spamChecker = spamChecker;
            _platoUserStore = platoUserStore;
            _topicStore = topicStore;
        }

        public async Task<ISpamOperatorResult<Topic>> ValidateModelAsync(ISpamOperatorContext<Topic> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Topic.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get user for topic
            var user = await _platoUserStore.GetByIdAsync(context.Model.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<Topic>();

            // User is OK
            var spamResult = await _spamChecker.CheckAsync(user);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);


        }

        public async Task<ISpamOperatorResult<Topic>> UpdateModelAsync(ISpamOperatorContext<Topic> context)
        {
            
            var validation = await ValidateModelAsync(context);

            // Not an operator of interest
            if (validation == null)
            {
                return null;
            }

            // If validation succeeded no need to perform further actions
            if (validation.Succeeded)
            {
                return null;
            }
            
            // Create result
            var result = new SpamOperatorResult<Topic>();
            
            // Flag as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                context.Model.IsSpam = true;
            }

            // Notify administrators of SPAM
            if (context.Operation.NotifyAdmin)
            {
                NotifyAdmins();
            }

            // Notify staff of SPAM
            if (context.Operation.NotifyStaff)
            {
                NotifyStaff();
            }
            
            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);
            
        }


        void NotifyAdmins()
        {

        }

        void NotifyStaff()
        {

        }

    }

}


