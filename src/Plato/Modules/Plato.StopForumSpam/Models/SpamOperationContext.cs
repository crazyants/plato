namespace Plato.StopForumSpam.Models
{

    public interface ISpamOperationContext<TModel> where TModel : class
    {
        TModel Model { get; set; }

        ISpamOperationType Operation { get; set; }

    }

    public class SpamOperationContext<TModel> : ISpamOperationContext<TModel> where TModel : class
    {

        public TModel Model { get; set; }

        public ISpamOperationType Operation { get; set; }

    }


}
