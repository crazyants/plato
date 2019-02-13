

using Plato.Internal.Abstractions;

namespace Plato.StopForumSpam.Models
{

    public interface ISpamOperationResult<TModel> where TModel : class
    {
        TModel Model { get; set; }

        ISpamOperationType Operation { get; set; }

    }

    public class SpamOperationResult<TModel> :  ISpamOperationResult<TModel> where TModel : class
    {

        public TModel Model { get; set; }

        public ISpamOperationType Operation { get; set; }


    }

}