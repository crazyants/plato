using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.SpamOperators
{
    public class RegistrationOperator : Plato.StopForumSpam.Services.ISpamOperatorProvider<User>
    {

   
        private readonly ISpamChecker _spamChecker;
     
        public RegistrationOperator(ISpamChecker spamChecker)
        {
            _spamChecker = spamChecker;
        }

        public async Task<ISpamOperatorResult<User>> ValidateModelAsync(ISpamOperatorContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Registration.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<User>();

            // If our result is OK simply return
            var spamResult = await _spamChecker.CheckAsync(context.Model);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        public async Task<ISpamOperatorResult<User>> UpdateModelAsync(ISpamOperatorContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Registration.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<User>();
                         
            // If our result is OK simply return
            var spamResult = await _spamChecker.CheckAsync(context.Model);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
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
