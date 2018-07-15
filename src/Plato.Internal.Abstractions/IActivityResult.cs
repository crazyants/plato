using System.Collections.Generic;

namespace Plato.Internal.Abstractions
{

    public interface IActivityResult<out TResponse> where TResponse : class
    {

        bool Succeeded { get; }

        TResponse Response { get; }

        IEnumerable<ActivityError> Errors { get; }

    }

    public class ActivityError
    {

        public ActivityError(string description) : this("", description)
        {
        }

        public ActivityError(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; set; }

        public string Description { get; set; }

    }


}
