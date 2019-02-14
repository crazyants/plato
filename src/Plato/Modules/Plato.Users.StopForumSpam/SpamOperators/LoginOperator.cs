using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;

namespace Plato.Users.StopForumSpam.SpamOperators
{

    public class LoginOperator : ISpamOperatorProvider<User>
    {

        private readonly ISpamChecker _spamChecker;
        
        public LoginOperator(ISpamChecker spamChecker)
        {
            _spamChecker = spamChecker;
        }

        public async Task<ISpamOperatorResult<User>> OperateAsync(ISpamOperatorContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Login.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<User>();
            
            // Spam checks returned false, return success
            // to indicate no further work is needed

            var spamResult = await _spamChecker.CheckAsync(context.Model);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Flag as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                context.Model.IsSpam = true;
            }

            // Notify administrators of SPAM
            NotifyAdmins();

            // Notify staff of SPAM
            NotifyStaff();

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


