using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Client.Services;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;
using Plato.StopForumSpam.Stores;

namespace Plato.Users.StopForumSpam.SpamOperations
{
    public class RegistrationOperation : ISpamOperationProvider<User>
    {
        
        private readonly ISpamChecker _spamChecker;
     
        public RegistrationOperation(ISpamChecker spamChecker)
        {
            _spamChecker = spamChecker;
        }

        public async Task<ICommandResult<User>> ExecuteAsync(ISpamOperationContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperationTypes.Registration.Name, StringComparison.Ordinal))
            {
                return null;
            }
            
            // Spam checks returned false, no further work needed
            if (!await _spamChecker.CheckAsync(context.Model))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<User>();

            // Flag as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                context.Model.IsSpam = true;
            }

            // Notify Admins

            // Notify Staff

            // Message

            // Allow Alter

            if (context.Operation.AllowAlter)
            {

            }


            return null;

        }
    }
}
