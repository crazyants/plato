using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Client.Models;
using System.Collections.Generic;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamCheckerResult : ICommandResultBase
    {
    }

    public class SpamCheckerResult : ISpamCheckerResult
    {
        
        public bool Succeeded { get; protected set; }

        public IEnumerable<CommandError> Errors { get; protected set; }

        public ISpamCheckerResult Success()
        {
            return new SpamCheckerResult()
            {
                Succeeded = true
            };
        }

        public ISpamCheckerResult Fail(IEnumerable<RequestType> requestTypes)
        {

            var errors = new List<CommandError>();
            foreach (var requestType in requestTypes)
            {
                switch (requestType)
                {
                    case RequestType.Username:
                        errors.Add(new CommandError(requestType.ToString(), "username"));
                        break;
                    case RequestType.EmailAddress:
                        errors.Add(new CommandError(requestType.ToString(), "email address"));
                        break;
                    case RequestType.IpAddress:
                        errors.Add(new CommandError(requestType.ToString(), "IP address"));
                        break;
                }
            }

            return new SpamCheckerResult()
            {
                Succeeded = false,
                Errors = errors
            };
        }
        
    }
    
}
