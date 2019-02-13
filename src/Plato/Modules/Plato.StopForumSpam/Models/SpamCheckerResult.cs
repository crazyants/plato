using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamCheckerResult : ICommandResultBase
    {
    }

    public class SpamCheckerResult : ISpamCheckerResult
    {

        private readonly List<CommandError> _errors = new List<CommandError>();

        public bool Succeeded { get; protected set; }

        public IEnumerable<CommandError> Errors => (IEnumerable<CommandError>)this._errors;
        
        public SpamCheckerResult()
        {
        }

        public ISpamCheckerResult Success()
        {
            return new SpamCheckerResult()
            {
                Succeeded = true
            };
        }
        public ISpamCheckerResult Fail(string message)
        {
            return new SpamCheckerResult()
            {
                Succeeded = false
            };
        }

        public void Error(RequestType requestType)
        {

            Succeeded = false;

            switch (requestType)
            {
                case RequestType.Username:
                    _errors.Add(new CommandError(requestType.ToString(), "username"));
                    break;
                case RequestType.EmailAddress:
                    _errors.Add(new CommandError(requestType.ToString(), "email address"));
                    break;
                case RequestType.IpAddress:
                    _errors.Add(new CommandError(requestType.ToString(), "IP address"));
                    break;
            }

        }

    }


}
