namespace Plato.StopForumSpam.Models
{
    public interface ISpamOperatorContext<TModel> where TModel : class
    {
        TModel Model { get; set; }

        ISpamOperation Operation { get; set; }

    }

}
