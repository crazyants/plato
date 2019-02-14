using System.Collections.Generic;
using System.Linq;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Models
{
    
    public class SpamCheckerResult : ISpamCheckerResult
    {

        public bool Succeeded => !Errors.Any();
        
        public IEnumerable<CommandError> Errors { get; protected set; }

        public IProxyResults Results { get; protected set; }

        public SpamCheckerResult()
        {
            Errors = new List<CommandError>();
            Results = new ProxyResults();
        }

        public ISpamCheckerResult Success()
        {
            return new SpamCheckerResult();
        }

        public ISpamCheckerResult Success(IProxyResults results)
        {
            return new SpamCheckerResult()
            {
                Results = results
            };
        }

        public ISpamCheckerResult Fail(IEnumerable<RequestType> requestTypes, IProxyResults results)
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
                Errors = errors,
                Results = results
            };
        }
        
    }
    
}
