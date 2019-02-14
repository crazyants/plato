using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class StopForumSpamViewModel
    {

        public int Id { get; set; }

        public bool IsNewUser { get; set; }

        public bool IsSpam { get; set; }

        public ISpamCheckerResult Checker { get; set; }

    }

}
