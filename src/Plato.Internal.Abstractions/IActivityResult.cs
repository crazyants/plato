using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{

    public interface IActivityResult<out TResponse> where TResponse : class
    {

        bool Succeeded { get; }

        TResponse Response { get; }

        IEnumerable<EntityError> Errors { get; }

    }

    public class EntityError
    {

        public EntityError(string description) : this("", description)
        {
        }

        public EntityError(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; set; }

        public string Description { get; set; }

    }


}
