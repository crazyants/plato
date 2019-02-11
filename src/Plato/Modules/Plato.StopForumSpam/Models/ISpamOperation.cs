namespace Plato.StopForumSpam.Models
{
    public interface ISpamOperation
    {

        string Name { get; set; }

        string Description { get; set; }

        string Category { get; set; }
        
        bool FlagAsSpam { get; set; }

        bool NotifyAdmin { get; set; }

        bool NotifyStaff { get; set; }

        bool AllowAlter { get; set; }
        
        string Message { get; set; }

    }



}
