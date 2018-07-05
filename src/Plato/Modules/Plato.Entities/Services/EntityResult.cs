using System;
using System.Collections.Generic;
using System.Linq;

namespace Plato.Entities.Services
{

    public interface IEntityResult
    {

        bool Succeeded { get; }

        object Response { get; }

        IEnumerable<EntityError> Errors { get; }
        
    }

    public class EntityResult : IEntityResult
    {

        private readonly List<EntityError> _errors = new List<EntityError>();
   
        public bool Succeeded { get; protected set; }

        public object Response { get; protected set; }
        
        public IEnumerable<EntityError> Errors => (IEnumerable<EntityError>)this._errors;

        public EntityResult Success(object response)
        {
            return new EntityResult()
            {
                Response = response,
                Succeeded = true
            };
        }

        public EntityResult Failed(params EntityError[] errors)
        {
            var entityResult = new EntityResult()
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
                    $"{(object) "Failed"} : {(object) string.Join(",", (IEnumerable<string>) this.Errors.Select<EntityError, string>((Func<EntityError, string>) (x => x.Code)).ToList<string>())}";
            }
                
            return "Succeeded";
        }

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
