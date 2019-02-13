namespace Plato.StopForumSpam.Models
{
    
    public class SpamOperatorContext<TModel> : ISpamOperatorContext<TModel> where TModel : class
    {

        public TModel Model { get; set; }

        public ISpamOperation Operation { get; set; }

    }
    
}
