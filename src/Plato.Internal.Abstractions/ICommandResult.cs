using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{

    public interface ICommandResult<out TResponse> where TResponse : class
    {

        bool Succeeded { get; }

        TResponse Response { get; }

        IEnumerable<CommandError> Errors { get; }

    }

}
