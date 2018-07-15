using System;
using System.Collections.Generic;
using System.Linq;

namespace Plato.Internal.Abstractions
{


    public class ActivityResult<TResponse> : IActivityResult<TResponse> where TResponse : class
    {

        private readonly List<ActivityError> _errors = new List<ActivityError>();

        public bool Succeeded { get; protected set; }

        public TResponse Response { get; protected set; }

        public IEnumerable<ActivityError> Errors => (IEnumerable<ActivityError>)this._errors;

        public ActivityResult<TResponse> Success()
        {
            return new ActivityResult<TResponse>()
            {
                Succeeded = true
            };
        }

        public ActivityResult<TResponse> Success(object response)
        {

            // No response object just return success
            if (response == null)
            {
                return Success();
            }

            // Cast our generic response object to expected type
            return new ActivityResult<TResponse>()
            {
                Response = (TResponse)Convert.ChangeType(response, typeof(TResponse)),
                Succeeded = true
            };
        }

        public ActivityResult<TResponse> Failed(string message)
        {
            var entityResult = new ActivityResult<TResponse>()
            {
                Succeeded = false
            };

            entityResult._errors.Add(new ActivityError(message));

            return entityResult;
        }


        public ActivityResult<TResponse> Failed(params ActivityError[] errors)
        {
            var entityResult = new ActivityResult<TResponse>()
            {
                Succeeded = false
            };
            if (errors != null)
                entityResult._errors.AddRange((IEnumerable<ActivityError>)errors);
            return entityResult;
        }

        public override string ToString()
        {
            if (!this.Succeeded)
            {
                return
                    $"{(object)"Failed"} : {(object)string.Join(",", (IEnumerable<string>)this.Errors.Select<ActivityError, string>((Func<ActivityError, string>)(x => x.Code)).ToList<string>())}";
            }

            return "Succeeded";
        }

    }


}
