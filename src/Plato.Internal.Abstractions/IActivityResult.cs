using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{

    public interface IActivityResult<out TResponse> where TResponse : class
    {

        bool Succeeded { get; }

        TResponse Response { get; }

        IEnumerable<ActivityError> Errors { get; }

    }

}
