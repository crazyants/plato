namespace Plato.StopForumSpam.Models
{
    public interface ISpamOperation
    {

        string Name { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string Category { get; set; }
        
        bool FlagAsSpam { get; set; }

        bool NotifyAdmin { get; set; }

        bool NotifyStaff { get; set; }

        bool CustomMessage { get; set; }
        
        string Message { get; set; }

    }
    
}
