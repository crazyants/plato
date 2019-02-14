using Plato.Internal.Abstractions;
using Plato.StopForumSpam.Client.Models;

namespace Plato.StopForumSpam.Models
{
    public interface ISpamCheckerResult : ICommandResultBase
    {

        IProxyResults Results { get; }

    }

}
