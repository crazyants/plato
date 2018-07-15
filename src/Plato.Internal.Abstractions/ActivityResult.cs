using System;
using System.Collections.Generic;
using System.Linq;

namespace Plato.Internal.Abstractions
{


    public class ActivityResult<TResponse> : IActivityResult<TResponse> where TResponse : class
    {

        private readonly List<EntityError> _errors = new List<EntityError>();

        public bool Succeeded { get; protected set; }

        public TResponse Response { get; protected set; }

        public IEnumerable<EntityError> Errors => (IEnumerable<EntityError>)this._errors;

        public ActivityResult<TResponse> Success()
        {
            return new ActivityResult<TResponse>()
            {
                Succeeded = true
            };
        }

        public ActivityResult<TResponse> Success(object response)
        {
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

            entityResult._errors.Add(new EntityError(message));

            return entityResult;
        }


        public ActivityResult<TResponse> Failed(params EntityError[] errors)
        {
            var entityResult = new ActivityResult<TResponse>()
            {
                Succeeded = false
            };
            if (errors != null)
                entityResult._errors.AddRange((IEnumerable<EntityError>)errors);
            return entityResult;
        }

        public override string ToString()
        {
            if (!this.Succeeded)
            {
                return
                    $"{(object)"Failed"} : {(object)string.Join(",", (IEnumerable<string>)this.Errors.Select<EntityError, string>((Func<EntityError, string>)(x => x.Code)).ToList<string>())}";
            }

            return "Succeeded";
        }

    }


}
