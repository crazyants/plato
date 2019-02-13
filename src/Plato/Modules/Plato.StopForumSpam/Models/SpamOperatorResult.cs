using System.Collections.Generic;
using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{
    
    public class SpamOperatorResult<TModel> : ISpamOperatorResult<TModel> where TModel : class
    {

        public TModel Response { get; protected set; }

        public ISpamOperation Operation { get; protected set; }

        private readonly List<CommandError> _errors = new List<CommandError>();
        
        public bool Succeeded { get; protected set; }

        public IEnumerable<CommandError> Errors => (IEnumerable<CommandError>)this._errors;
        
        public SpamOperatorResult<TModel> Success(TModel response)
        {
            return new SpamOperatorResult<TModel>()
            {
                Response = response,
                Succeeded = true
            };
        }
        
        public SpamOperatorResult<TModel> Failed(TModel model, ISpamOperation operation)
        {
            return new SpamOperatorResult<TModel>()
            {
                Response = model,
                Operation = operation,
                Succeeded = false
            };
        }

        public SpamOperatorResult<TModel> Error(string message)
        {
            var result = new SpamOperatorResult<TModel>()
            {
                Succeeded = false
            };

            result._errors.Add(new CommandError(message));

            return result;
        }

        public SpamOperatorResult<TModel> Error(params CommandError[] errors)
        {
            var result = new SpamOperatorResult<TModel>()
            {
                Succeeded = false
            };
            if (errors != null)
            {
                result._errors.AddRange((IEnumerable<CommandError>)errors);
            }
                
            return result;
        }
        
    }

}