using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Users.StopForumSpam.SpamOperations
{

    public class LoginOperation : ISpamOperationProvider<User>
    {

        public Task<ICommandResult<User>> ExecuteAsync(ISpamOperationContext<User> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperationTypes.Login.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<User>();

            return null;

        }
    }

}


