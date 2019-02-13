using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class UserSpamViewModel
    {

        public int Id { get; set; }

        public ISpamCheckerResult SpamCheckerResult { get; set; }
        
        public bool IsNewUser { get; set; }

    }

}
