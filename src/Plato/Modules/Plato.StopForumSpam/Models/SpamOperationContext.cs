namespace Plato.StopForumSpam.Models
{

    public interface ISpamOperatorContext<TModel> where TModel : class
    {
        TModel Model { get; set; }

        ISpamOperation Operation { get; set; }

    }

    public class SpamOperatorContext<TModel> : ISpamOperatorContext<TModel> where TModel : class
    {

        public TModel Model { get; set; }

        public ISpamOperation Operation { get; set; }

    }


}
