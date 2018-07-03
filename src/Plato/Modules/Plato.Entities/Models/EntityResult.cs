using System;
using System.Collections.Generic;
using System.Linq;

namespace Plato.Entities.Models
{

    public class EntityError
    {

        public EntityError(string message)
        : this("", message)
        {
         
        }

        public EntityError(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public string Code { get; set; }

        public string Message { get; set; }

    }

    public interface IEntityResult
    {

        bool Succeeded { get; }

        IEnumerable<EntityError> Errors { get; }

        EntityResult Success { get; }

    }

    public class EntityResult : IEntityResult
    {

        private readonly List<EntityError> _errors = new List<EntityError>();
   
        public bool Succeeded { get; protected set; }
        
        public IEnumerable<EntityError> Errors => (IEnumerable<EntityError>)this._errors;

        public EntityResult Success
        {
            get
            {
                var entityResult = new EntityResult()
                {
                    Succeeded = true
                };
                return entityResult;
            }
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

}
